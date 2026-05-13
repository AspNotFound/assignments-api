using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetSubmissionsByAssignmentHandler
(
    IAssignmentReadRepository assignment,
    ISubmissionReadRepository submission,
    IUser user,
    ICourseService course
)
{
    private readonly ISubmissionReadRepository _submission = submission;
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly ICourseService _course = course;

    public async Task<Result<IReadOnlyCollection<Domain.Aggregates.Submission>>> HandleAsync(GetSubmissionsByAssignmentQuery request)
    {
        var courseId = await _assignment.GetCourseIdByAssignmentIdAsync(request.AssignmentId);
        if (courseId == null)
            return Result<IReadOnlyCollection<Domain.Aggregates.Submission>>.Failure(FailureType.NotFound, "Assignment not found.");

        var userHasAccess = _user.IsAdmin() || _user.IsTeacher() && await _course.IsTeacherOfCourseAsync(_user.UserId, courseId);

        if (!userHasAccess)
            return Result<IReadOnlyCollection<Domain.Aggregates.Submission>>.Failure(FailureType.Unauthorized, "You do not have access to these submissions.");

        var submissions = await _submission.GetAllByAssignmentIdAsync(request.AssignmentId);
        return Result<IReadOnlyCollection<Domain.Aggregates.Submission>>.Success(submissions);
    }
}