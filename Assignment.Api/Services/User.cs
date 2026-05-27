using System.Security.Claims;
using Assignment.Application.Abstractions;

namespace Assignment.Api.Services;

public class User(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            userId ??= (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            userId ??= (_httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value);

            if (userId is null)
            {
                throw new InvalidOperationException("User ID claim not found in the token.");
            }

            return userId;
        }
    }

    public bool IsAdmin()
    {
        var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ??
                      _httpContextAccessor.HttpContext?.User.IsInRole("Administrator") ??
                      _httpContextAccessor.HttpContext?.User.IsInRole("admin") ?? false;
        return isAdmin;
    }

    public bool IsStudent()
    {
        var isStudent = _httpContextAccessor.HttpContext?.User?.IsInRole("Student") ??
                        _httpContextAccessor.HttpContext?.User.IsInRole("student") ?? false;
        return isStudent;
    }

    public bool IsTeacher()
    {
        var isTeacher = _httpContextAccessor.HttpContext?.User?.IsInRole("Teacher") ??
                        _httpContextAccessor.HttpContext?.User.IsInRole("teacher") ?? false;
        return isTeacher;
    }
}