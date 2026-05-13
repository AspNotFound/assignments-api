using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Submissions.Handlers;

public class GetSubmissionByIdHandler
(
    ISubmissionReadRepository submission,
    IAssignmentReadRepository assignment,
    IUser user,
    ICourseService course,
    IEnrollmentService enrollment
)
{
    private readonly ISubmissionReadRepository _submission = submission;
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly ICourseService _course = course;
    private readonly IEnrollmentService _enrollment = enrollment;

    public async Task<Result<Domain.Aggregates.Submission>> HandleAsync(GetSubmissionByIdQuery request)
    {
        var assignmentId = await _submission.GetAssignmentIdBySubmissionIdAsync(request.SubmissionId);
        if (assignmentId == null)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.NotFound, "Submission not found.");

        var courseId = await _assignment.GetCourseIdByAssignmentIdAsync(assignmentId.Value);
        if (courseId == null)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.NotFound, "Assignment not found.");

        var authorId = await _submission.GetAuthorIdBySubmissionIdAsync(request.SubmissionId);

        var userHasAccess = _user.IsAdmin() ||
            _user.IsStudent() && authorId == _user.UserId ||
            _user.IsTeacher() && await _course.IsTeacherOfCourseAsync(_user.UserId, courseId);

        if (!userHasAccess)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.Unauthorized, "You do not have access to this submission.");

        var entity = await _submission.GetByIdAsync(request.SubmissionId);
        if (entity == null)
            return Result<Domain.Aggregates.Submission>.Failure(FailureType.NotFound, "Submission not found.");

        return Result<Domain.Aggregates.Submission>.Success(entity);
    }
}
