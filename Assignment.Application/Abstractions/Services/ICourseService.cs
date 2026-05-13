namespace Assignment.Application.Abstractions.Services;

public interface ICourseService
{
    public Task<bool> IsTeacherOfCourseAsync(string userId, string courseId);
    public Task<bool> ExistsAsync(string courseId);
}