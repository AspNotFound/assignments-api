using Assignment.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace Assignment.Api.Services;

public class Course(HttpClient http, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : ICourseService
{
    private readonly HttpClient _http = http;
    private readonly string _courseServiceBaseUrl = configuration.GetSection("Microservices")["CourseServiceBaseUrl"] ?? throw new InvalidOperationException("CourseServiceBaseUrl is not configured.");
    private readonly string _teacherServiceBaseUrl = configuration.GetSection("Microservices")["TeacherServiceBaseUrl"] ?? throw new InvalidOperationException("TeacherServiceBaseUrl is not configured.");
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<bool> IsTeacherOfCourseAsync(string userId, string courseId)
    {
        var httpContextAuthorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_teacherServiceBaseUrl}/api/course-teachers/course/{courseId}")
        {
            Headers =
            {
                { "Authorization", httpContextAuthorizationHeader }
            }
        };

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CourseTeacher>() ?? throw new InvalidOperationException();
        return result.TeacherId == userId;
    }

    public async Task<bool> ExistsAsync(string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_courseServiceBaseUrl}/api/courses")
        {
            Headers =
            {
                { "Authorization", _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault() ?? string.Empty }
            }
        };
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var allCourses = await response.Content.ReadFromJsonAsync<List<CourseEntity>>() ?? throw new InvalidOperationException();
        return allCourses.Any(c => c.Id.ToString() == courseId);
    }
}

public class CourseTeacher
{
    public Guid Id { get; set; }
    public int CourseId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
}

public class CourseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}
