using Assignment.Application.Abstractions;

namespace Assignment.Application.Security.Authorization;

public abstract class AuthorizationPolicyBase(IUser user)
{
    protected readonly IUser _user = user;
    private readonly Dictionary<string, Dictionary<string, bool>> _permissionsCache = [];

    // Caches the permission and returns the value.
    protected bool CachePermission(string courseId, string permissionName, bool value)
    {
        if (!_permissionsCache.ContainsKey(courseId))
        {
            _permissionsCache[courseId] = [];
        }

        _permissionsCache[courseId][permissionName] = value;
        return value;
    }

    protected bool TryGetCachedPermission(string courseId, string permissionName, out bool value)
    {
        value = false;
        return _permissionsCache.TryGetValue(courseId, out var permissions) && permissions.TryGetValue(permissionName, out value);
    }
}