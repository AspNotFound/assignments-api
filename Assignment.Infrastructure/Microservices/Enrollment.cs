using Assignment.Application.Abstractions.Services;

namespace Assignment.Infrastructure.Microservices;

public class Enrollment : IEnrollmentService
{
    public Enrollment()
    {
        // Initialize any necessary resources or dependencies here
    }

#warning implement these
    public async Task<bool> IsStudentOfCourseAsync(string userId, string courseId)
    {
        return true;
    }
}