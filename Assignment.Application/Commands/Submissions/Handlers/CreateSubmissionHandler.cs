using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;
using Assignment.Domain.Aggregates;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class CreateSubmissionHandler
(
    IAssignmentRepository assignment,
    ISubmissionRepository submission,
    IUser user,
    ISubmissionReadRepository submissionReadRepository,
    SubmissionAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly ISubmissionRepository _submission = submission;
    private readonly IUser _user = user;
    private readonly SubmissionAuthorizationPolicy _authorizationPolicy = authorizationPolicy;
    private readonly ISubmissionReadRepository _submissionReadRepository = submissionReadRepository;

    public async Task<Result<Dto<Submission, SubmissionPermissions>>> HandleAsync(CreateSubmissionCommand request)
    {
        var userIsAllowedToCreateSubmission = await _authorizationPolicy.CanCreateSubmissionOnAssignmentAsync(request.AssignmentId);
        if (!userIsAllowedToCreateSubmission)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.Unauthorized, "User is not authorized to create a submission for this assignment.");
        }

        var assignment = await _assignment.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, $"Assignment with ID {request.AssignmentId} not found.");
        }

        var userHasAlreadySubmittedSubmission = await _submissionReadRepository.UserSubmissionExistsAsync(request.AssignmentId, _user.UserId);

        List<Domain.Entities.Attachment> attachments;
        Submission submission;
        try
        {
            attachments = [.. request.Attachments.Select(a => Domain.Entities.Attachment.Create(a.Name, a.FileName, a.Link))];
            submission = assignment.CreateSubmission(userHasAlreadySubmittedSubmission, _user.UserId, request.Content, attachments);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.DomainError, $"Failed to create submission: {ex.Message}");
        }

        _submission.Add(submission);
        await _submission.SaveChangesAsync();

        var permissions = await SubmissionPermissions.CreateAsync(submission.Id, _authorizationPolicy);
        var dto = Dto<Submission, SubmissionPermissions>.Create(submission, permissions);
        return Result<Dto<Submission, SubmissionPermissions>>.Success(dto);
    }
}