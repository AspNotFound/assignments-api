using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetAuthorAssignmentSubmissionHandler
(
    IAssignmentReadRepository assignment,
    ISubmissionReadRepository submission,
    IUser user,
    ICourseService course
)
{
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly ICourseService _course = course;
    private readonly ISubmissionReadRepository _submission = submission;

    public async Task<Result<Domain.Aggregates.Submission>> HandleAsync(GetAuthorAssignmentSubmissionQuery request)
    {
        var assignmentCourseId = await _assignment.GetCourseIdByAssignmentIdAsync(request.AssignmentId);
        if (assignmentCourseId == null)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.NotFound, "Assignment not found.");

        var userHasAccess = _user.IsAdmin() ||
            _user.IsStudent() && request.AuthorId == _user.UserId ||
            _user.IsTeacher() && await _course.IsTeacherOfCourseAsync(_user.UserId, assignmentCourseId);

        if (!userHasAccess)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.Unauthorized, "You do not have access to this submission.");

        var entity = await _submission.GetByAssignmentAndAuthorAsync(request.AssignmentId, request.AuthorId);
        if (entity == null)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.NotFound, "Submission not found.");

        return Result<Domain.Aggregates.Submission>.Success(entity);
    }
}