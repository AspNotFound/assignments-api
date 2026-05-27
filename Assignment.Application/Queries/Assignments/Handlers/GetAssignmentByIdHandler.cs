using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Assignments.Handlers;

public class GetAssignmentByIdHandler
(
    IAssignmentReadRepository assignment,
    IUser user,
    AssignmentAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly AssignmentAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.Assignment, AssignmentPermissions>>> HandleAsync(GetAssignmentByIdQuery request)
    {
        var useCanAccessAssignment = await _authorizationPolicy.CanAccessAssignmentAsync(request.AssignmentId);
        if (!useCanAccessAssignment)
            return Result<Dto<Domain.Aggregates.Assignment, AssignmentPermissions>>.Failure(FailureType.Unauthorized, "You do not have access to this assignment.");

        var entity = await _assignment.GetByIdAsync(request.AssignmentId);
        if (entity == null)
            return Result<Dto<Domain.Aggregates.Assignment, AssignmentPermissions>>.Failure(FailureType.NotFound, "Assignment not found.");

        var permissions = new AssignmentPermissions
        (
            Edit: await _authorizationPolicy.CanModifyAssignmentAsync(request.AssignmentId),
            CreateSubmission: await _authorizationPolicy.CanCreateSubmissionForAssignmentAsync(request.AssignmentId),
            ViewSubmissions: await _authorizationPolicy.CanViewSubmissionsForAssignmentAsync(request.AssignmentId)
        );

        var dto = Dto<Domain.Aggregates.Assignment, AssignmentPermissions>.Create(entity, permissions);
        return Result<Dto<Domain.Aggregates.Assignment, AssignmentPermissions>>.Success(dto);
    }
}