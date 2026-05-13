using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class UpdateAssignmentHandler
{
    private readonly IAssignmentRepository _assignmentRepository;

    public UpdateAssignmentHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Result<Domain.Aggregates.Assignment>> HandleAsync(UpdateAssignmentCommand request)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.Id);
        if (assignment == null)
        {
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.NotFound, $"Assignment with id {request.Id} not found.");
        }

        try
        {
            assignment.Rename(request.Name);
            assignment.UpdateDescription(request.Description);
            assignment.DelayDeadline(request.Deadline);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.DomainError, $"Failed to update assignment: {ex.Message}");
        }

        await _assignmentRepository.UpdateAsync(assignment);
        return Result<Domain.Aggregates.Assignment>.Success(assignment);
    }
}