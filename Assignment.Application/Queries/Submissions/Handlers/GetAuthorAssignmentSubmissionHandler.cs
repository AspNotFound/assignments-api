using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetAuthorAssignmentSubmissionHandler
(
    IAssignmentReadRepository assignment,
    ISubmissionReadRepository submission,
    SubmissionAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly ISubmissionReadRepository _submission = submission;
    private readonly SubmissionAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>> HandleAsync(GetAuthorAssignmentSubmissionQuery request)
    {
        var submission = await _submission.GetByAssignmentAndAuthorAsync(request.AssignmentId, request.AuthorId);
        if (submission == null)
            return Result<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, "Submission not found.");

        var userHasAccess = await _authorizationPolicy.CanAccessSubmissionAsync(submission.Id);
        if (!userHasAccess)
            return Result<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>.Failure(FailureType.Unauthorized, "You do not have access to this submission.");

        var permissions = await SubmissionPermissions.CreateAsync(submission.Id, _authorizationPolicy);
        var dto = Dto<Domain.Aggregates.Submission, SubmissionPermissions>.Create(submission, permissions);
        return Result<Dto<Domain.Aggregates.Submission, SubmissionPermissions>>.Success(dto);
    }
}