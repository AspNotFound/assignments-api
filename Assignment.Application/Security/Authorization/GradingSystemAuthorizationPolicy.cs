using Assignment.Application.Abstractions;

namespace Assignment.Application.Security.Authorization;

public class GradingSystemAuthorizationPolicy(IUser user) : AuthorizationPolicyBase(user)
{
    public bool CanAccessGradingSystem()
    {
        const string permissionName = "Access";
        if (TryGetCachedPermission("GradingSystem", permissionName, out var canAccess))
        {
            return canAccess;
        }

        return CachePermission("GradingSystem", permissionName, true);
    }

    public bool CanModifyGradingSystem()
    {
        const string permissionName = "Modify";
        if (TryGetCachedPermission("GradingSystem", permissionName, out var canModify))
        {
            return canModify;
        }

        return CachePermission("GradingSystem", permissionName, IsAdminOrTeacher());
    }

    private bool IsAdminOrTeacher()
    {
        return _user.IsAdmin() || _user.IsTeacher();
    }
}