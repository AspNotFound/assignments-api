using Assignment.Application.Abstractions.Services;

namespace Assignment.Infrastructure.Microservices;

public class Enrollment : IEnrollmentService
{
    public Enrollment()
    {
        // Initialize any necessary resources or dependencies here
    }

    public async Task<bool> IsStudentOfCourseAsync(string userId, string courseId)
    {
        // Implement logic to check if the user is a student of the course
        return await Task.FromResult(true); // Placeholder implementation
    }
}