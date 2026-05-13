using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.Assignments.Handlers;

public class GetAllAssignmentsByCourseHandler
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

    public async Task<Result<IReadOnlyCollection<Domain.Aggregates.Assignment>>> HandleAsync(GetAllAssignmentsByCourseQuery request)
    {
        var userHasAccess = _user.IsAdmin() ||
            _user.IsStudent() && await _enrollment.IsStudentOfCourseAsync(_user.UserId, request.CourseId) ||
            _user.IsTeacher() && await _course.IsTeacherOfCourseAsync(_user.UserId, request.CourseId);

        if (!userHasAccess)
            return Result<IReadOnlyCollection<Domain.Aggregates.Assignment>>.Failure(FailureType.Unauthorized, "You do not have access to the assignments of this course.");

        var entities = await _assignment.GetAllByCourseIdAsync(request.CourseId);
        return Result<IReadOnlyCollection<Domain.Aggregates.Assignment>>.Success(entities);
    }
}