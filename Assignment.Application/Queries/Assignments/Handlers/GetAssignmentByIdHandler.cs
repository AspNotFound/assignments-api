using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Assignments.Handlers;

public class GetAssignmentByIdHandler
(
    IAssignmentReadRepository assignment,
    IUser user,
    ICourseService course,
    IEnrollmentService enrollment
)
{
    private readonly IAssignmentReadRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly ICourseService _course = course;
    private readonly IEnrollmentService _enrollment = enrollment;

    public async Task<Result<Domain.Aggregates.Assignment>> HandleAsync(GetAssignmentByIdQuery request)
    {
        var courseId = await _assignment.GetCourseIdByAssignmentIdAsync(request.AssignmentId);
        if (courseId == null)
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.NotFound, "Assignment not found.");

        var userHasAccess = _user.IsAdmin() ||
            _user.IsStudent() && await _enrollment.IsStudentOfCourseAsync(_user.UserId, courseId) ||
            _user.IsTeacher() && await _course.IsTeacherOfCourseAsync(_user.UserId, courseId);

        if (!userHasAccess)
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.Unauthorized, "You do not have access to this assignment.");

        var entity = await _assignment.GetByIdAsync(request.AssignmentId);
        return entity == null
            ? Result<Domain.Aggregates.Assignment>.Failure(FailureType.NotFound, "Assignment not found.")
            : Result<Domain.Aggregates.Assignment>.Success(entity);
    }
}