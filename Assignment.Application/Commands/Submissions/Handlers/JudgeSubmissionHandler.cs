using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;
using Assignment.Domain.Services;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class JudgeSubmissionHandler
(
    ISubmissionRepository submission,
    IAssignmentRepository assignment,
    IUser user,
    IGradingSystemRepository gradingSystem,
    ICourseService courseService
)
{
    private readonly ISubmissionRepository _submission = submission;
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly ICourseService _courseService = courseService;

    public async Task<Result> Handle(JudgeSubmissionCommand request)
    {
        var submission = await _submission.GetByIdAsync(request.SubmissionId);
        if (submission == null)
        {
            return Result.Failure(FailureType.NotFound, $"Submission with ID {request.SubmissionId} not found.");
        }

        var assignment = await _assignment.GetByIdAsync(submission.AssignmentId);
        if (assignment == null)
        {
            return Result.Failure(FailureType.NotFound, $"Assignment with ID {submission.AssignmentId} not found.");
        }

        var userIsAllowedToJudgeSubmission = _user.IsAdmin() || (_user.IsTeacher() && await _courseService.IsTeacherOfCourseAsync(_user.UserId, assignment.CourseId));

        if (!userIsAllowedToJudgeSubmission)
        {
            return Result.Failure(FailureType.Unauthorized, "User is not authorized to judge this submission.");
        }

        var gradingSystem = await _gradingSystem.GetByIdAsync(assignment.GradingSystemId);
        if (gradingSystem == null)
        {
            return Result.Failure(FailureType.NotFound, $"Grading system with ID {assignment.GradingSystemId} not found.");
        }

        var grade = gradingSystem.Grades.FirstOrDefault(g => g.Id == request.GradingSystemGradeId);
        if (grade == null)
        {
            return Result.Failure(FailureType.NotFound, $"Grade with ID {request.GradingSystemGradeId} not found in grading system with ID {assignment.GradingSystemId}.");
        }

        try
        {
            SubmissionDomainService.Judge(assignment, gradingSystem, submission, grade, _user.UserId, request.Feedback);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result.Failure(FailureType.DomainError, $"Failed to judge submission: {ex.Message}");
        }

        await _submission.UpdateAsync(submission);

        return Result.Success();
    }
}