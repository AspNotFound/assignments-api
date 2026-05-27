using Assignment.Application.Dtos;
using Assignment.Application.Security.Permissions;
using Assignment.Domain.Aggregates;

namespace Assignment.Application.Extensions;

public static class SubmissionExtensions
{
    public static Dto<object, SubmissionPermissions> ToDto(this Submission submission, SubmissionPermissions permissions)
    {
        var dto = new
        {
            submission.Id,
            submission.AssignmentId,
            submission.AuthorId,
            submission.Content,
            submission.CreatedAt,
            submission.ModifiedAt,
            Judgement = permissions.ViewJudgement || permissions.Judge ? submission.Judgement : null,
            submission.Attachments
        };

        return Dto<object, SubmissionPermissions>.Create(dto, permissions);
    }
}