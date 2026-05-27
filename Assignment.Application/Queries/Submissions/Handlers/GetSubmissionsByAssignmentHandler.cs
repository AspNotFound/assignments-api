using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetSubmissionsByAssignmentHandler
(
    ISubmissionReadRepository submission,
    SubmissionAuthorizationPolicy authorizationPolicy
)
{
    private readonly ISubmissionReadRepository _submission = submission;
    private readonly SubmissionAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<IReadOnlyCollection<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>>> HandleAsync(GetSubmissionsByAssignmentQuery request)
    {
        var userHasAccess = await _authorizationPolicy.CanAccessAssignmentSubmissionsAsync(request.AssignmentId);
        if (!userHasAccess)
            return Result<IReadOnlyCollection<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>>.Failure(FailureType.Unauthorized, "You do not have access to these submissions.");

        var submissions = await _submission.GetAllByAssignmentIdAsync(request.AssignmentId);
        var dtos = await Task.WhenAll(submissions.Select(async submission => new Dto<Domain.Aggregates.Submission, SubmissionPermissions>
        (
            submission,
            await SubmissionPermissions.CreateAsync(submission.Id, _authorizationPolicy)
        )));

        return Result<IReadOnlyCollection<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>>.Success(dtos);
    }
}