using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Assignments.Handlers;

public class GetAllAssignmentsHandler
(
    IAssignmentReadRepository assignment,
    AssignmentAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly AssignmentAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<IReadOnlyCollection<Dto<Domain.Aggregates.Assignment, Permissions>>>> HandleAsync(GetAllAssignments request)
    {
        if (!string.IsNullOrEmpty(request.CourseId))
        {
            var userHasAccess = await _authorizationPolicy.CanAccessCourseAssignmentsAsync(request.CourseId);
            if (!userHasAccess)
                return Result<IReadOnlyCollection<Dto<Domain.Aggregates.Assignment, Permissions>>>.Failure(FailureType.Unauthorized, "You do not have access to the assignments of this course.");
        }

        var entities = await _assignment.GetAll(request.CourseId);

        var dtos = (await Task.WhenAll(entities.Select(async e => Dto<Domain.Aggregates.Assignment, Permissions>.Create(e, new Permissions(Edit: await _authorizationPolicy.CanModifyAssignmentAsync(e.Id)))))).ToList();
        return Result<IReadOnlyCollection<Dto<Domain.Aggregates.Assignment, Permissions>>>.Success(dtos);
    }
}