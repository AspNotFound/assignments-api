using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class CreateGradingSystemHandler(IGradingSystemRepository gradingSystem, IUser user, GradingSystemAuthorizationPolicy authorizationPolicy)
{
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly IUser _user = user;
    private readonly GradingSystemAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>> HandleAsync(CreateGradingSystemCommand request)
    {
        var userIsAllowedToModifyGradingSystems = _authorizationPolicy.CanModifyGradingSystem();
        if (!userIsAllowedToModifyGradingSystems)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.Unauthorized, "User is not authorized to create grading systems.");
        }

        var gradingSystemGrades = request.Grades.Select(g => Domain.Entities.GradingSystemGrade.Create(g.Name, g.IsPassingGrade, g.Order)).ToList();
        Domain.Aggregates.GradingSystem gradingSystem;
        try
        {
            gradingSystem = Domain.Aggregates.GradingSystem.Create(request.Name, gradingSystemGrades);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.DomainError, $"Failed to create grading system: {ex.Message}");
        }

        _gradingSystem.Add(gradingSystem);
        await _gradingSystem.SaveChangesAsync();
        var dto = Dto<Domain.Aggregates.GradingSystem, Permissions>.Create(gradingSystem, new Permissions(Edit: userIsAllowedToModifyGradingSystems));
        return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Success(dto);
    }
}