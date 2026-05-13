using Assignment.Domain.Entities;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Aggregates;

public class Assignment
{
    public static Assignment Create(bool gradingSystemExists, Guid gradingSystemId, string courseId, string name, string description, DateTimeOffset deadline)
    {
        if (!gradingSystemExists)
        {
            throw new DomainException("Cannot create an assignment for a non-existing grading system.");
        }

        return new Assignment(gradingSystemId, courseId, name, description, deadline);
    }

    public static Assignment Hydrate(Guid id, Guid gradingSystemId, string courseId, string name, string description, DateTimeOffset deadline)
    {
        return new Assignment(id, gradingSystemId, courseId, name, description, deadline);
    }

    private Assignment(Guid gradingSystemId, string courseId, string name, string description, DateTimeOffset deadline)
    {
        if (!IsValidName(name, out var validationErrorMessageName))
        {
            throw new DomainException(validationErrorMessageName);
        }

        if (!IsValidDeadline(deadline, out var validationErrorMessageDeadline))
        {
            throw new DomainException(validationErrorMessageDeadline);
        }

        if (!IsValidDescription(description, out var validationErrorMessageDescription))
        {
            throw new DomainException(validationErrorMessageDescription);
        }

        Id = Guid.CreateVersion7();
        GradingSystemId = gradingSystemId;
        CourseId = courseId;
        Name = name;
        Description = description;
        Deadline = deadline;
    }

    private Assignment(Guid id, Guid gradingSystemId, string courseId, string name, string description, DateTimeOffset deadline)
    {
        Id = id;
        GradingSystemId = gradingSystemId;
        CourseId = courseId;
        Name = name;
        Description = description;
        Deadline = deadline;
    }

    public Guid Id { get; }
    public Guid GradingSystemId { get; }
    public string CourseId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset Deadline { get; private set; }

    public Submission CreateSubmission(bool userHasAlreadySubmitted, string authorId, string content, IReadOnlyCollection<Attachment> attachments)
    {
        if (!IsAllowedToCreateSubmissions())
        {
            throw new DomainException("Cannot create a submission for this assignment.");
        }

        if (userHasAlreadySubmitted)
        {
            throw new DomainException("User has already submitted for this assignment.");
        }

        return Submission.Create(Id, authorId, content, attachments);
    }

    public void Rename(string newName)
    {
        if (!IsValidName(newName, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Name = newName;
    }

    public void UpdateDescription(string newDescription)
    {
        if (!IsValidDescription(newDescription, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Description = newDescription;
    }

    public void DelayDeadline(DateTimeOffset newDeadline)
    {
        if (!IsValidDeadline(newDeadline, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Deadline = newDeadline;
    }

    private static bool IsValidName(string name, out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            validationErrorMessage = "Assignment name cannot be null or whitespace.";
            return false;
        }

        return true;
    }

    private static bool IsValidDeadline(DateTimeOffset deadline, out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        if (deadline <= DateTimeOffset.UtcNow)
        {
            validationErrorMessage = "Assignment deadline must be in the future.";
            return false;
        }

        return true;
    }

    private static bool IsValidDescription(string description, out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        if (string.IsNullOrWhiteSpace(description))
        {
            validationErrorMessage = "Assignment description cannot be null or whitespace.";
            return false;
        }
        return true;
    }

    internal bool IsAllowedToCreateSubmissions()
    {
        return Deadline > DateTimeOffset.UtcNow;
    }

    internal bool IsAllowedToEditSubmissions()
    {
        return IsAllowedToCreateSubmissions();
    }
}