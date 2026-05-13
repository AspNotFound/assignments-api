using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class CreateGradingSystemHandler(IGradingSystemRepository gradingSystem, IUser user)
{
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly IUser _user = user;

    public async Task<Result<Domain.Aggregates.GradingSystem>> HandleAsync(CreateGradingSystemCommand request)
    {
        var userIsAllowedToCreateGradingSystem = _user.IsAdmin() || _user.IsTeacher();
        if (!userIsAllowedToCreateGradingSystem)
        {
            return Result<Domain.Aggregates.GradingSystem>.Failure(FailureType.Unauthorized, "Only admins and teachers can create grading systems.");
        }

        var gradingSystemGrades = request.Grades.Select(g => Domain.Entities.GradingSystemGrade.Create(g.Name, g.IsPassingGrade, g.Order)).ToList();
        Domain.Aggregates.GradingSystem gradingSystem;
        try
        {
            gradingSystem = Domain.Aggregates.GradingSystem.Create(request.Name, gradingSystemGrades);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Domain.Aggregates.GradingSystem>.Failure(FailureType.DomainError, $"Failed to create grading system: {ex.Message}");
        }
        await _gradingSystem.AddAsync(gradingSystem);
        return Result<Domain.Aggregates.GradingSystem>.Success(gradingSystem);
    }
}