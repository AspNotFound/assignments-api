using Assignment.Application.Abstractions.Services;

namespace Assignment.Infrastructure.Microservices;

public class Course : ICourseService
{
    public async Task<bool> IsTeacherOfCourseAsync(string userId, string courseId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(string courseId)
    {
        throw new NotImplementedException();
    }
}