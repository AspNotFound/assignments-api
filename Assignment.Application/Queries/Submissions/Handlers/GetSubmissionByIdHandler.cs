using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Extensions;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetSubmissionByIdHandler
(
    ISubmissionReadRepository submission,
    IAssignmentReadRepository assignment,
    SubmissionAuthorizationPolicy authorizationPolicy
)
{
    private readonly ISubmissionReadRepository _submission = submission;
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly SubmissionAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<object, SubmissionPermissions>>> HandleAsync(GetSubmissionByIdQuery request)
    {
        var userHasAccess = await _authorizationPolicy.CanAccessSubmissionAsync(request.SubmissionId);

        if (!userHasAccess)
            return Result<Dto<object, SubmissionPermissions>>.Failure(FailureType.Unauthorized, "You do not have access to this submission.");

        var entity = await _submission.GetByIdAsync(request.SubmissionId);
        if (entity == null)
            return Result<Dto<object, SubmissionPermissions>>.Failure(FailureType.NotFound, "Submission not found.");

        var permissions = new SubmissionPermissions
        (
            Edit: await _authorizationPolicy.CanModifySubmissionAsync(entity.Id),
            Judge: await _authorizationPolicy.CanJudgeSubmissionAsync(entity.Id),
            ViewJudgement: await _authorizationPolicy.CanViewJudgementAsync(entity.Id)
        );

        var dto = entity.ToDto(permissions);
        return Result<Dto<object, SubmissionPermissions>>.Success(dto);
    }
}