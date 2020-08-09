using Oxide.Ext.RustApi.Primitives.Models;
using System;
using System.Linq;

namespace Oxide.Ext.RustApi.Business.Common
{
    /// <summary>
    /// Base route class
    /// </summary>
    internal abstract class RouteBase
    {
        /// <summary>
        /// System admin user permissions name
        /// </summary>
        public static string SystemAdminPermission = "admin";

        /// <summary>
        /// Validate user permissions.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="requiredPermissions">Required permission.</param>
        protected bool IsUserHasAccess(ApiUserInfo user, params string[] requiredPermissions)
        {
            // if no permissions configured, any authorized user has access
            if (!requiredPermissions.Any()) return true;

            // if user is anonymous and there is required permissions, then deny access
            if (user.IsAnonymous) return false;

            // admin permission to allow everything
            if (user.Permissions.Any(x => x.Equals(SystemAdminPermission, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            // try to find required permissions
            var result = user.Permissions.Intersect(requiredPermissions, StringComparer.InvariantCultureIgnoreCase).Any();
            return result;
        }
    }
}
