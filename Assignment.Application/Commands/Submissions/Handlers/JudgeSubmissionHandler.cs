using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;
using Assignment.Domain.Aggregates;
using Assignment.Domain.Services;

namespace Assignment.Application.Commands.Submissions.Handlers;

public class JudgeSubmissionHandler
(
    ISubmissionRepository submission,
    IAssignmentRepository assignment,
    IUser user,
    IGradingSystemRepository gradingSystem,
    SubmissionAuthorizationPolicy authorizationPolicy
)
{
    private readonly ISubmissionRepository _submission = submission;
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly SubmissionAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Submission, SubmissionPermissions>>> HandleAsync(JudgeSubmissionCommand request)
    {
        var userIsAllowedToJudgeSubmission = await _authorizationPolicy.CanJudgeSubmissionAsync(request.SubmissionId);
        if (!userIsAllowedToJudgeSubmission)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.Unauthorized, "User is not authorized to judge this submission.");
        }

        var submission = await _submission.GetByIdAsync(request.SubmissionId);
        if (submission == null)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, $"Submission with ID {request.SubmissionId} not found.");
        }

        var assignment = await _assignment.GetByIdAsync(submission.AssignmentId);
        if (assignment == null)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, $"Assignment with ID {submission.AssignmentId} not found.");
        }

        var gradingSystem = await _gradingSystem.GetByIdAsync(assignment.GradingSystemId);
        if (gradingSystem == null)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, $"Grading system with ID {assignment.GradingSystemId} not found.");
        }

        var grade = gradingSystem.Grades.FirstOrDefault(g => g.Id == request.GradingSystemGradeId);
        if (grade == null)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.NotFound, $"Grade with ID {request.GradingSystemGradeId} not found in grading system with ID {assignment.GradingSystemId}.");
        }

        try
        {
            SubmissionDomainService.Judge(assignment, gradingSystem, submission, grade, _user.UserId, request.Feedback);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<Dto<Submission, SubmissionPermissions>>.Failure(FailureType.DomainError, $"Failed to judge submission: {ex.Message}");
        }

        _submission.Update(submission);
        await _submission.SaveChangesAsync();


        var permissions = await SubmissionPermissions.CreateAsync(request.SubmissionId, _authorizationPolicy);
        return Result<Dto<Submission, SubmissionPermissions>>.Success(new Dto<Submission, SubmissionPermissions>(submission, permissions));
    }
}