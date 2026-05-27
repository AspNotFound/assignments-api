using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Services;

namespace Assignment.Application.Security.Authorization;

public class SubmissionAuthorizationPolicy
(
    IUser user,
    ICourseService courseService,
    ISubmissionReadRepository submissionReadRepository,
    IAssignmentReadRepository assignmentReadRepository,
    IEnrollmentService enrollmentService
) : AuthorizationPolicyBase(user)
{
    private readonly ICourseService _courseService = courseService;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;
    private readonly ISubmissionReadRepository _submissionReadRepository = submissionReadRepository;
    private readonly IAssignmentReadRepository _assignmentReadRepository = assignmentReadRepository;

    public async Task<bool> CanCreateSubmissionOnAssignmentAsync(Guid assignmentId)
    {
        const string permissionName = "CreateSubmissionOnAssignment";
        var cacheKey = assignmentId.ToString();
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
            var courseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
                return CachePermission(cacheKey, permissionName, false);
            var isStudentOfCourse = await _enrollmentService.IsStudentOfCourseAsync(_user.UserId, courseId);
            return CachePermission(cacheKey, permissionName, isStudentOfCourse);
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanAccessAssignmentSubmissionsAsync(Guid assignmentId)
    {
        const string permissionName = "AccessAssignmentSubmissions";
        var cacheKey = assignmentId.ToString();
        if (TryGetCachedPermission(cacheKey, permissionName, out var canAccess))
        {
            return canAccess;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            var courseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(assignmentId);
            if (courseId == null)
                return CachePermission(cacheKey, permissionName, false);

            var isTeacherOfCourse = await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId);
            return CachePermission(cacheKey, permissionName, isTeacherOfCourse);
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanAccessSubmissionAsync(Guid submissionId)
    {
        const string permissionName = "AccessSubmission";
        var cacheKey = submissionId.ToString();
        if (TryGetCachedPermission(cacheKey, permissionName, out var canAccess))
        {
            return canAccess;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            var assignmentId = await _submissionReadRepository.GetAssignmentIdBySubmissionIdAsync(submissionId);
            if (!assignmentId.HasValue)
                return CachePermission(cacheKey, permissionName, false);

            var courseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(assignmentId.Value);
            if (courseId == null)
                return CachePermission(cacheKey, permissionName, false);

            var isTeacherOfCourse = await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId);
            return CachePermission(cacheKey, permissionName, isTeacherOfCourse);
        }

        if (_user.IsStudent())
        {
            var authorId = await _submissionReadRepository.GetAuthorIdBySubmissionIdAsync(submissionId);
            var isAuthor = authorId == _user.UserId;
            return CachePermission(cacheKey, permissionName, isAuthor);
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanModifySubmissionAsync(Guid submissionId)
    {
        const string permissionName = "ModifySubmission";
        var cacheKey = submissionId.ToString();
        if (TryGetCachedPermission(cacheKey, permissionName, out var canModify))
        {
            return canModify;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsStudent())
        {
            var authorId = await _submissionReadRepository.GetAuthorIdBySubmissionIdAsync(submissionId);
            var isAuthor = authorId == _user.UserId;
            return CachePermission(cacheKey, permissionName, isAuthor);
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    public async Task<bool> CanJudgeSubmissionAsync(Guid submissionId)
    {
        const string permissionName = "JudgeSubmission";
        var cacheKey = submissionId.ToString();
        if (TryGetCachedPermission(cacheKey, permissionName, out var canJudge))
        {
            return canJudge;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            var assignmentId = await _submissionReadRepository.GetAssignmentIdBySubmissionIdAsync(submissionId);
            if (!assignmentId.HasValue)
                return CachePermission(cacheKey, permissionName, false);

            var courseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(assignmentId.Value);
            if (courseId == null)
                return CachePermission(cacheKey, permissionName, false);

            var isTeacherOfCourse = await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId);
            return CachePermission(cacheKey, permissionName, isTeacherOfCourse);
        }

        return CachePermission(cacheKey, permissionName, false);
    }

    internal async Task<bool> CanViewJudgementAsync(Guid id)
    {
        const string permissionName = "ViewSubmissionJudgement";
        var cacheKey = id.ToString();
        if (TryGetCachedPermission(cacheKey, permissionName, out var canViewJudgement))
        {
            return canViewJudgement;
        }

        if (_user.IsAdmin())
        {
            return CachePermission(cacheKey, permissionName, true);
        }

        if (_user.IsTeacher())
        {
            var assignmentId = await _submissionReadRepository.GetAssignmentIdBySubmissionIdAsync(id);
            if (!assignmentId.HasValue)
                return CachePermission(cacheKey, permissionName, false);

            var courseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(assignmentId.Value);
            if (courseId == null)
                return CachePermission(cacheKey, permissionName, false);

            var isTeacherOfCourse = await _courseService.IsTeacherOfCourseAsync(_user.UserId, courseId);
            return CachePermission(cacheKey, permissionName, isTeacherOfCourse);
        }

        var authorId = await _submissionReadRepository.GetAuthorIdBySubmissionIdAsync(id);
        var isAuthor = authorId == _user.UserId;
        return CachePermission(cacheKey, permissionName, isAuthor);
    }
}