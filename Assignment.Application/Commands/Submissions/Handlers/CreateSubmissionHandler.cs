using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;
using Assignment.Domain.Aggregates;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class CreateSubmissionHandler
(
    IAssignmentRepository assignment,
    ISubmissionRepository submission,
    IUser user,
    IEnrollmentService enrollmentService
)
{
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly ISubmissionRepository _submission = submission;
    private readonly IUser _user = user;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;

    public async Task<Result<Submission>> HandleAsync(CreateSubmissionCommand request)
    {
        var assignment = await _assignment.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
        {
            return Result<Submission>.Failure(FailureType.NotFound, $"Assignment with ID {request.AssignmentId} not found.");
        }

        var userIsAllowedToCreateSubmission = _user.IsAdmin() || (_user.IsStudent() && await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, assignment.CourseId));

        if (!userIsAllowedToCreateSubmission)
        {
            return Result<Submission>.Failure(FailureType.Unauthorized, "User is not authorized to create a submission for this assignment.");
        }

        var userHasAlreadySubmitted = await _submission.UserSubmissionExistsAsync(request.AssignmentId, request.AuthorId);

        List<Domain.Entities.Attachment> attachments;
        Submission submission;

        try
        {
            attachments = [.. request.Attachments.Select(a => Domain.Entities.Attachment.Create(a.Name, a.FileName, a.Link))];
            submission = assignment.CreateSubmission(userHasAlreadySubmitted, request.AuthorId, request.Content, attachments);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Submission>.Failure(FailureType.DomainError, $"Failed to create submission: {ex.Message}");
        }

        await _submission.AddAsync(submission);
        return Result<Submission>.Success(submission);
    }
}