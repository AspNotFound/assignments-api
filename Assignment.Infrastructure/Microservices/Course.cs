using Assignment.Application.Abstractions.Services;

namespace Assignment.Infrastructure.Microservices;

public class Course : ICourseService
{
    #warning Implement these.
    public async Task<bool> IsTeacherOfCourseAsync(string userId, string courseId)
    {
        return true;
    }

    public async Task<bool> ExistsAsync(string courseId)
    {
        return true;
    }
}