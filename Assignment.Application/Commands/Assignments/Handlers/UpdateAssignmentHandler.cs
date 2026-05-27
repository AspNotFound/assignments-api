using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class UpdateAssignmentHandler(IAssignmentRepository assignmentRepository, AssignmentAuthorizationPolicy authorizationPolicy)
{
    private readonly IAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly AssignmentAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.Assignment, Permissions>>> HandleAsync(UpdateAssignmentCommand request)
    {
        var userIsAllowedToUpdateAssignment = await _authorizationPolicy.CanModifyAssignmentAsync(request.Id);
        if (!userIsAllowedToUpdateAssignment)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.Unauthorized, "User is not authorized to update assignment.");
        }

        var assignment = await _assignmentRepository.GetByIdAsync(request.Id);
        if (assignment == null)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.NotFound, $"Assignment with id {request.Id} not found.");
        }

        try
        {
            assignment.Rename(request.Name);
            assignment.UpdateDescription(request.Description);
            assignment.DelayDeadline(request.Deadline);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.DomainError, $"Failed to update assignment: {ex.Message}");
        }

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync();
        var dto = Dto<Domain.Aggregates.Assignment, Permissions>.Create(assignment, new Permissions(Edit: userIsAllowedToUpdateAssignment));
        return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Success(dto);
    }
}