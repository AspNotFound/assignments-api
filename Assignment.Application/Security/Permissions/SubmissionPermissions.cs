using Assignment.Application.Security.Authorization;

namespace Assignment.Application.Security.Permissions;

public record SubmissionPermissions(bool Edit, bool Judge, bool ViewJudgement) : Permissions(Edit)
{
    public static async Task<SubmissionPermissions> CreateAsync(Guid submissionId, SubmissionAuthorizationPolicy authorizationPolicy)
    {
        var canEdit = await authorizationPolicy.CanModifySubmissionAsync(submissionId);
        var canJudge = await authorizationPolicy.CanJudgeSubmissionAsync(submissionId);
        var canViewJudgement = await authorizationPolicy.CanViewJudgementAsync(submissionId);

        return new SubmissionPermissions(canEdit, canJudge, canViewJudgement);
    }
}