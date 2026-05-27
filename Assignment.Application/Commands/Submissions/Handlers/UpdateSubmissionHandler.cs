using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;
using Assignment.Domain.Aggregates;
using Assignment.Domain.Exceptions;
using Assignment.Domain.Services;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class UpdateSubmissionHandler
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

    public async Task<Result<Submission>> HandleAsync(UpdateSubmissionCommand request)
    {
        var submission = await _submission.GetByIdAsync(request.Id);
        if (submission == null)
        {
            return Result<Submission>.Failure(FailureType.NotFound, $"Submission with ID {request.Id} not found.");
        }

        var assignment = await _assignment.GetByIdAsync(submission.AssignmentId);
        if (assignment == null)
        {
            return Result<Submission>.Failure(FailureType.NotFound, $"Assignment with ID {submission.AssignmentId} not found.");
        }

        var userIsAllowedToUpdateSubmission = _user.IsAdmin() || (_user.IsStudent() && await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, assignment.CourseId) && submission.AuthorId == _user.UserId);
        if (!userIsAllowedToUpdateSubmission)
        {
            return Result<Submission>.Failure(FailureType.Unauthorized, "User is not authorized to update a submission for this assignment.");
        }

        try
        {
            var newAttachments = request.Attachments.Select(a => Domain.Entities.Attachment.Create(a.Name, a.FileName, a.Link)).ToList();
            SubmissionDomainService.Update(assignment, submission, request.Content, newAttachments);
        }
        catch (DomainException ex)
        {
            return Result<Submission>.Failure(FailureType.DomainError, ex.Message);
        }

        _submission.Update(submission);
        await _submission.SaveChangesAsync();
        return Result<Submission>.Success(submission);
    }
}