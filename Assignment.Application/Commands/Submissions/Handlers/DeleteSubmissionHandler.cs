using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class DeleteSubmissionHandler(ISubmissionRepository submission, IUser user)
{
    private readonly ISubmissionRepository _submission = submission;
    private readonly IUser _user = user;

    public async Task<Result> HandleAsync(DeleteSubmissionCommand request)
    {
        var submission = await _submission.GetByIdAsync(request.SubmissionId);
        if (submission is null)
        {
            return Result.Failure(FailureType.NotFound, $"Submission with ID {request.SubmissionId} not found.");
        }

        var userIsAllowedToDeleteSubmission = _user.IsAdmin() || submission.AuthorId == _user.UserId;

        if (!userIsAllowedToDeleteSubmission)
        {
            return Result.Failure(FailureType.Unauthorized, "User is not authorized to delete this submission.");
        }

        if (!submission.IsDeletable)
        {
            return Result.Failure(FailureType.DomainError, "Submission cannot be deleted.");
        }

        _submission.Delete(submission.Id);
        await _submission.SaveChangesAsync();
        return Result.Success();
    }
}