using Assignment.Application.Abstractions.Services;

namespace Assignment.Api.Services;

public class Enrollment(HttpClient http, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : IEnrollmentService
{
    private readonly HttpClient _http = http;
    private readonly string _enrollmentServiceBaseUrl = configuration.GetSection("Microservices")["EnrollmentServiceBaseUrl"] ?? throw new InvalidOperationException("EnrollmentServiceBaseUrl is not configured.");
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<bool> IsStudentOfCourseAsync(string userId, string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_enrollmentServiceBaseUrl}/api/enrollment/student/{userId}")
        {
            Headers =
            {
                { "Authorization", _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault() ?? string.Empty }
            }
        };

        var result = await _http.SendAsync(request);
        result.EnsureSuccessStatusCode();
        var enrollment = await result.Content.ReadFromJsonAsync<EnrollmentResponse>() ?? throw new InvalidOperationException();
        return enrollment != null && enrollment.classId.ToString() == courseId;
    }
}

public record EnrollmentResponse
(
    int id,
    Guid studentId,
    int classId,
    DateTime enrollmentDate
);