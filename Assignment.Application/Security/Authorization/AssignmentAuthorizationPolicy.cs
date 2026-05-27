using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;

namespace Assignment.Application.Security.Authorization;

public class AssignmentAuthorizationPolicy(IUser user, ICourseService courseService, IEnrollmentService enrollmentService, IAssignmentReadRepository readRepository) : AuthorizationPolicyBase(user)
{
    private readonly ICourseService _courseService = courseService;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;
    private readonly IAssignmentReadRepository _readRepository = readRepository;

    public async Task<bool> CanCreateAssignmentForCourseAsync(string courseId)
    {
        const string permissionName = "CreateAssignment";
        var cacheKey = $"Course-{courseId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canCreate))
        {
            return canCreate;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            return CachePermission(cacheKey, permissionName, await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId));
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanAccessAssignmentAsync(Guid assignmentId)
    {
        const string permissionName = "Access";
        var cacheKey = $"Assignment-{assignmentId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canAccess))
        {
            return canAccess;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsStudent() || _user.IsTeacher())
        {
            var courseId = await _readRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
            {
                return CachePermission(cacheKey, permissionName, false);
            }
            if (_user.IsStudent())
            {
                return CachePermission(cacheKey, permissionName, await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, courseId));
            }
            else
            {
                return CachePermission(cacheKey, permissionName, await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId));
            }
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanModifyAssignmentAsync(Guid assignmentId)
    {
        const string permissionName = "Modify";
        var cacheKey = $"Assignment-{assignmentId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canModify))
        {
            return canModify;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {

            var courseId = await _readRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
            {
                return CachePermission(cacheKey, permissionName, false);
            }

            return CachePermission(cacheKey, permissionName, await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId));
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanAccessCourseAssignmentsAsync(string courseId)
    {
        const string permissionName = "AccessCourseAssignments";
        var cacheKey = $"Course-{courseId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canAccess))
        {
            return canAccess;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsStudent() || _user.IsTeacher())
        {
            if (_user.IsStudent())
            {
                return CachePermission(cacheKey, permissionName, await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, courseId));
            }
            else
            {
                return CachePermission(cacheKey, permissionName, await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId));
            }
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanCreateSubmissionForAssignmentAsync(Guid assignmentId)
    {
        const string permissionName = "CreateSubmission";
        var cacheKey = $"Assignment-{assignmentId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canCreate))
        {
            return canCreate;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsStudent())
        {
            var courseId = await _readRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
            {
                return CachePermission(cacheKey, permissionName, false);
            }
            return CachePermission(cacheKey, permissionName, await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, courseId));
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanViewSubmissionsForAssignmentAsync(Guid assignmentId)
    {
        const string permissionName = "ViewSubmissions";
        var cacheKey = $"Assignment-{assignmentId}-Permission-{permissionName}";

        if (TryGetCachedPermission(cacheKey, permissionName, out var canView))
        {
            return canView;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            var courseId = await _readRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
            {
                return CachePermission(cacheKey, permissionName, false);
            }
            return CachePermission(cacheKey, permissionName, await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId));
        }

        return CachePermission(cacheKey, permissionName, false);
    }
}