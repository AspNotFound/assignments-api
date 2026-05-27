using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class DeleteAssignmentHandler
(
    IAssignmentReadRepository assignmentReadRepository,
    IAssignmentRepository assignmentRepository,
    AssignmentAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentReadRepository _assignmentReadRepository = assignmentReadRepository;
    private readonly IAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly AssignmentAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result> HandleAsync(DeleteAssignmentCommand request)
    {
        var userIsAllowedToDeleteAssignment = await _authorizationPolicy.CanModifyAssignmentAsync(request.AssignmentId);
        if (!userIsAllowedToDeleteAssignment)
        {
            return Result.Failure(FailureType.Unauthorized, "User is not authorized to delete assignment.");
        }

        _assignmentRepository.Delete(request.AssignmentId);
        await _assignmentRepository.SaveChangesAsync();
        return Result.Success();
    }
}