using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;
using Assignment.Domain.Aggregates;
using Assignment.Domain.Exceptions;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class UpdateGradingSystemHandler(IGradingSystemRepository gradingSystem, GradingSystemAuthorizationPolicy authorizationPolicy)
{
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly GradingSystemAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>> HandleAsync(UpdateGradingSystemCommand request)
    {
        var userIsAllowedToModifyGradingSystem = _authorizationPolicy.CanModifyGradingSystem();
        if (!userIsAllowedToModifyGradingSystem)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.Unauthorized, "User is not authorized to modify grading systems.");
        }

        var gradingSystem = await _gradingSystem.GetByIdAsync(request.Id) ?? throw new InvalidOperationException($"Grading system with ID {request.Id} not found.");

        try
        {
            gradingSystem.Rename(request.Name);
        }
        catch (DomainException de)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.DomainError, $"Failed to rename grading system: {de.Message}");
        }

        var gradeModifications = gradingSystem.Grades
        .Where(existingGrade => request.Grades.All(g => g.Id != existingGrade.Id))
        .Select(g => new RemoveGradingSystemGradeModification(g.Id))
        .Cast<IGradingSystemGradeModification>()
        .Union
        (
            request.Grades
            .Where(g => g.Id != null)
            .Select(g => new UpdateGradingSystemGradeModification(g.Id!.Value, g.Name, g.IsPassingGrade, g.Order))
        )
        .Union
        (
            request.Grades
            .Where(g => g.Id == null)
            .Select(g => new AddGradingSystemGradeModification(Domain.Entities.GradingSystemGrade.Create(g.Name, g.IsPassingGrade, g.Order)))
        ).ToList();

        gradingSystem.ChangeGrades(gradeModifications);
        _gradingSystem.Update(gradingSystem);
        await _gradingSystem.SaveChangesAsync();

        var dto = Dto<Domain.Aggregates.GradingSystem, Permissions>.Create(gradingSystem, new Permissions(Edit: userIsAllowedToModifyGradingSystem));
        return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Success(dto);
    }
}