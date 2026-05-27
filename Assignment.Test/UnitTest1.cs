using Assignment.Domain.Aggregates;
using Assignment.Domain.Entities;
using Assignment.Infrastructure.Ef;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.ReadRepositories;
using Assignment.Infrastructure.Ef.Repositories;
using Microsoft.EntityFrameworkCore;
using static Assignment.Test.TestFactory;

namespace Assignment.Test;

public class InfrastructureRepositoryTests
{
    [Test]
    public async Task GradingSystemRepository_AddUpdateDelete_WorksWithInMemoryDb()
    {
        var dbName = $"grading-system-test-{Guid.NewGuid():N}";
        using var context = CreateContext(dbName);
        var repository = new GradingSystemRepository(context);

        var id = Guid.NewGuid();
        var initialFailId = Guid.NewGuid();
        var initialPassId = Guid.NewGuid();
        var gradingSystem = CreateGradingSystem(
            id,
            "Original",
            [
                GradingSystemGrade.Hydrate(initialFailId, "Fail", false, 1),
                GradingSystemGrade.Hydrate(initialPassId, "Pass", true, 2)
            ]
        );

        repository.Add(gradingSystem);
        await repository.SaveChangesAsync();

        Assert.That(await repository.ExistsAsync(id), Is.True);

        var loaded = await repository.GetByIdAsync(id);
        Assert.That(loaded, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(loaded!.Name, Is.EqualTo("Original"));
            Assert.That(loaded.Grades.Count, Is.EqualTo(2));
        });

        var updated = CreateGradingSystem(
            id,
            "Updated",
            [
                GradingSystemGrade.Hydrate(initialPassId, "Pass-Updated", true, 3),
                GradingSystemGrade.Hydrate(Guid.NewGuid(), "Excellent", true, 4),
                GradingSystemGrade.Hydrate(Guid.NewGuid(), "Fail", false, 1)
            ]
        );

        _ = await repository.GetByIdAsync(id);
        repository.Update(updated);
        await repository.SaveChangesAsync();

        await using var verifyContext = CreateContext(dbName);
        var verifyRepository = new GradingSystemRepository(verifyContext);
        var afterUpdate = await verifyRepository.GetByIdAsync(id);
        Assert.That(afterUpdate, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(afterUpdate!.Name, Is.EqualTo("Updated"));
            Assert.That(afterUpdate.Grades.Count, Is.EqualTo(3));
            Assert.That(afterUpdate.Grades.Any(g => g.Name == "Pass-Updated" && g.Order == 3), Is.True);
            Assert.That(afterUpdate.Grades.Any(g => g.Name == "Excellent"), Is.True);
        });

        _ = await verifyRepository.GetByIdAsync(id);
        verifyRepository.Delete(id);
        await verifyRepository.SaveChangesAsync();

        await using var afterDeleteContext = CreateContext(dbName);
        var afterDeleteRepository = new GradingSystemRepository(afterDeleteContext);
        Assert.That(await afterDeleteRepository.ExistsAsync(id), Is.False);
    }

    [Test]
    public async Task AssignmentRepository_AddGetUpdateDelete_WorksWithInMemoryDb()
    {
        using var context = CreateContext();
        var repository = new AssignmentRepository(context);

        var gradingSystem = CreateGradingSystem(Guid.NewGuid(), "Course Grades");
        context.GradingSystems.Add(Mapping.ConvertDomainModelToEntity(gradingSystem));
        await context.SaveChangesAsync();

        var assignmentId = Guid.NewGuid();
        var assignment = CreateAssignment(assignmentId, gradingSystem.Id, "course-1", "Assignment 1", "Initial description");

        repository.Add(assignment);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(assignmentId);
        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.Name, Is.EqualTo("Assignment 1"));

        var updated = CreateAssignment(assignmentId, gradingSystem.Id, "course-1", "Assignment 1 Updated", "Updated description");
        _ = await repository.GetByIdAsync(assignmentId);
        repository.Update(updated);
        await repository.SaveChangesAsync();

        var afterUpdate = await repository.GetByIdAsync(assignmentId);
        Assert.That(afterUpdate, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(afterUpdate!.Name, Is.EqualTo("Assignment 1 Updated"));
            Assert.That(afterUpdate.Description, Is.EqualTo("Updated description"));
        });

        _ = await repository.GetByIdAsync(assignmentId);
        repository.Delete(assignmentId);
        await repository.SaveChangesAsync();

        Assert.That(await repository.GetByIdAsync(assignmentId), Is.Null);
    }

    [Test]
    public async Task SubmissionRepository_AddUpdateAndQueries_WorkWithInMemoryDb()
    {
        var dbName = $"submission-test-{Guid.NewGuid():N}";
        using var context = CreateContext(dbName);
        var repository = new SubmissionRepository(context);

        var gradingSystem = CreateGradingSystem(Guid.NewGuid(), "Submission Grades");
        context.GradingSystems.Add(Mapping.ConvertDomainModelToEntity(gradingSystem));

        var assignment = CreateAssignment(Guid.NewGuid(), gradingSystem.Id, "course-2", "Assignment S", "Submission assignment");
        context.Assignments.Add(Mapping.ConvertDomainModelToEntity(assignment));
        await context.SaveChangesAsync();

        var submissionId = Guid.NewGuid();
        var firstAttachmentId = Guid.NewGuid();
        var initialSubmission = Submission.Hydrate(
            submissionId,
            assignment.Id,
            "student-1",
            "Initial content",
            [Attachment.Hydrate(firstAttachmentId, "report", "report.pdf", "https://example.com/report.pdf")],
            DateTimeOffset.UtcNow.AddMinutes(-10),
            DateTimeOffset.UtcNow.AddMinutes(-10),
            null
        );

        repository.Add(initialSubmission);
        await repository.SaveChangesAsync();

        Assert.That(await repository.UserSubmissionExistsAsync(assignment.Id, "student-1"), Is.True);

        var passGrade = gradingSystem.Grades.First(g => g.IsPassingGrade);
        var updatedSubmission = Submission.Hydrate(
            submissionId,
            assignment.Id,
            "student-1",
            "Updated content",
            [Attachment.Hydrate(Guid.NewGuid(), "solution", "solution.pdf", "https://example.com/solution.pdf")],
            DateTimeOffset.UtcNow.AddMinutes(-10),
            DateTimeOffset.UtcNow,
            Judgement.Hydrate(Guid.NewGuid(), "judge-1", passGrade, "Looks good", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
        );

        _ = await repository.GetByIdAsync(submissionId);
        repository.Update(updatedSubmission);
        await repository.SaveChangesAsync();

        await using var verifyContext = CreateContext(dbName);
        var verifyRepository = new SubmissionRepository(verifyContext);
        var loaded = await verifyRepository.GetByIdAsync(submissionId);
        Assert.That(loaded, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(loaded!.Content, Is.EqualTo("Updated content"));
            Assert.That(loaded.Attachments.Count, Is.EqualTo(1));
            Assert.That(loaded.Attachments.Single().Name, Is.EqualTo("solution"));
            Assert.That(loaded.Judgement, Is.Not.Null);
            Assert.That(loaded.Judgement!.JudgeId, Is.EqualTo("judge-1"));
            Assert.That(loaded.Judgement.Grade.Id, Is.EqualTo(passGrade.Id));
        });
    }
}

public class InfrastructureReadRepositoryTests
{
    [Test]
    public async Task AssignmentReadRepository_ReturnsByIdCourseAndCourseAssignments()
    {
        using var context = CreateContext();
        var repository = new AssignmentReadRepository(context);

        var gradingSystem = CreateGradingSystem(Guid.NewGuid(), "Read Grading");
        context.GradingSystems.Add(Mapping.ConvertDomainModelToEntity(gradingSystem));

        var a1 = CreateAssignment(Guid.NewGuid(), gradingSystem.Id, "course-read", "A1", "D1");
        var a2 = CreateAssignment(Guid.NewGuid(), gradingSystem.Id, "course-read", "A2", "D2");
        var other = CreateAssignment(Guid.NewGuid(), gradingSystem.Id, "other-course", "A3", "D3");

        context.Assignments.AddRange(
            Mapping.ConvertDomainModelToEntity(a1),
            Mapping.ConvertDomainModelToEntity(a2),
            Mapping.ConvertDomainModelToEntity(other)
        );
        await context.SaveChangesAsync();

        var byId = await repository.GetByIdAsync(a1.Id);
        var courseId = await repository.GetCourseIdByAssignmentIdAsync(a1.Id);
        var byCourse = await repository.GetAllByCourseIdAsync("course-read");

        Assert.Multiple(() =>
        {
            Assert.That(byId, Is.Not.Null);
            Assert.That(courseId, Is.EqualTo("course-read"));
            Assert.That(byCourse.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GradingSystemReadRepository_ReturnsAllAndById()
    {
        using var context = CreateContext();
        var repository = new GradingSystemReadRepository(context);

        var gs1 = CreateGradingSystem(Guid.NewGuid(), "G1");
        var gs2 = CreateGradingSystem(Guid.NewGuid(), "G2");

        context.GradingSystems.AddRange(
            Mapping.ConvertDomainModelToEntity(gs1),
            Mapping.ConvertDomainModelToEntity(gs2)
        );
        await context.SaveChangesAsync();

        var all = await repository.GetAllAsync();
        var byId = await repository.GetByIdAsync(gs2.Id);

        Assert.Multiple(() =>
        {
            Assert.That(all.Count, Is.EqualTo(2));
            Assert.That(byId, Is.Not.Null);
            Assert.That(byId!.Name, Is.EqualTo("G2"));
        });
    }

    [Test]
    public async Task SubmissionReadRepository_ReturnsAllQueryMethods()
    {
        using var context = CreateContext();
        var repository = new SubmissionReadRepository(context);

        var gradingSystem = CreateGradingSystem(Guid.NewGuid(), "Read Submission Grades");
        context.GradingSystems.Add(Mapping.ConvertDomainModelToEntity(gradingSystem));

        var assignment = CreateAssignment(Guid.NewGuid(), gradingSystem.Id, "course-3", "Assignment 3", "Desc");
        context.Assignments.Add(Mapping.ConvertDomainModelToEntity(assignment));
        await context.SaveChangesAsync();

        var submission = Submission.Hydrate(
            Guid.NewGuid(),
            assignment.Id,
            "author-read",
            "Body",
            [Attachment.Hydrate(Guid.NewGuid(), "doc", "doc.txt", "https://example.com/doc.txt")],
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddMinutes(-5),
            null
        );

        context.Submissions.Add(Mapping.ConvertDomainModelToEntity(submission));
        await context.SaveChangesAsync();

        var byId = await repository.GetByIdAsync(submission.Id);
        var byPair = await repository.GetByAssignmentAndAuthorAsync(assignment.Id, "author-read");
        var allByAssignment = await repository.GetAllByAssignmentIdAsync(assignment.Id);
        var authorId = await repository.GetAuthorIdBySubmissionIdAsync(submission.Id);
        var assignmentId = await repository.GetAssignmentIdBySubmissionIdAsync(submission.Id);
        var exists = await repository.UserSubmissionExistsAsync(assignment.Id, "author-read");

        Assert.Multiple(() =>
        {
            Assert.That(byId, Is.Not.Null);
            Assert.That(byPair, Is.Not.Null);
            Assert.That(allByAssignment.Count, Is.EqualTo(1));
            Assert.That(authorId, Is.EqualTo("author-read"));
            Assert.That(assignmentId, Is.EqualTo(assignment.Id));
            Assert.That(exists, Is.True);
        });
    }
}

file static class TestFactory
{
    public static AssignmentContext CreateContext(string? databaseName = null)
    {
        var resolvedDatabaseName = databaseName ?? $"assignment-test-db-{Guid.NewGuid():N}";
        var options = new DbContextOptionsBuilder<AssignmentContext>()
            .UseInMemoryDatabase(resolvedDatabaseName)
            .Options;

        return new AssignmentContext(options);
    }

    public static GradingSystem CreateGradingSystem(Guid id, string name)
    {
        return CreateGradingSystem(
            id,
            name,
            [
                GradingSystemGrade.Hydrate(Guid.NewGuid(), $"{name}-Fail", false, 1),
                GradingSystemGrade.Hydrate(Guid.NewGuid(), $"{name}-Pass", true, 2)
            ]
        );
    }

    public static GradingSystem CreateGradingSystem(Guid id, string name, IReadOnlyCollection<GradingSystemGrade> grades)
    {
        return GradingSystem.Hydrate(id, name, grades);
    }

    public static Assignment.Domain.Aggregates.Assignment CreateAssignment(Guid id, Guid gradingSystemId, string courseId, string name, string description)
    {
        return Assignment.Domain.Aggregates.Assignment.Hydrate(
            id,
            gradingSystemId,
            courseId,
            name,
            description,
            DateTimeOffset.UtcNow.AddDays(7)
        );
    }
}
