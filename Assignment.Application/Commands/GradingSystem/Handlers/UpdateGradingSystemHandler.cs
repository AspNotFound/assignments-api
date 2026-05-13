using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class UpdateGradingSystemHandler(IGradingSystemRepository gradingSystem, IUser user)
{
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly IUser _user = user;

    public async Task<Result<Domain.Aggregates.GradingSystem>> HandleAsync(UpdateGradingSystemCommand request)
    {
        var userIsAllowedToUpdateGradingSystem = _user.IsAdmin() || _user.IsTeacher();
        if (!userIsAllowedToUpdateGradingSystem)
        {
            return Result<Domain.Aggregates.GradingSystem>.Failure(FailureType.Unauthorized, "Only admins and teachers can update grading systems.");
        }

        var gradingSystem = await _gradingSystem.GetByIdAsync(request.GradingSystemId) ?? throw new InvalidOperationException($"Grading system with ID {request.GradingSystemId} not found.");

        gradingSystem.Rename(request.Name);

        foreach (var newGrade in request.Grades)
        {
            if (newGrade.Id == null)
            {
                try
                {
                    gradingSystem.AddGrade(Domain.Entities.GradingSystemGrade.Create(newGrade.Name, newGrade.IsPassingGrade, newGrade.Order));
                }
                catch (Domain.Exceptions.DomainException ex)
                {
                    return Result<Domain.Aggregates.GradingSystem>.Failure(FailureType.DomainError, $"Failed to add grade: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    gradingSystem.RenameGrade(newGrade.Id.Value, newGrade.Name);
                    gradingSystem.ChangeGradePassingStatus(newGrade.Id.Value, newGrade.IsPassingGrade);
                }
                catch (Domain.Exceptions.DomainException ex)
                {
                    return Result<Domain.Aggregates.GradingSystem>.Failure(FailureType.DomainError, $"Failed to update grade: {ex.Message}");
                }
            }
        }

        await _gradingSystem.UpdateAsync(gradingSystem);
        return Result<Domain.Aggregates.GradingSystem>.Success(gradingSystem);
    }
}