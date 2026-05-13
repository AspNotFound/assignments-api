namespace Assignment.Application.Abstractions.Services;

public interface IEnrollmentService
{
    public Task<bool> IsStudentOfCourseAsync(string userId, string courseId);
}
