using System;
using System.Linq;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Common
{
    /// <summary>
    /// Base route class
    /// </summary>
    internal abstract class RouteBase
    {
        /// <summary>
        /// Validate user permissions.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="requiredPermissions">Required permission.</param>
        protected bool IsUserHasAccess(ApiUserInfo user, params string[] requiredPermissions)
        {
            // if no permissions configured, any authorized user has access
            if (!requiredPermissions.Any()) return true;

            // admin permission to allow everything
            const string adminAccessPermission = "admin";
            if (user.Permissions.Any(x => x.Equals(adminAccessPermission, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            // try to find required permissions
            var result = user.Permissions.Intersect(requiredPermissions, StringComparer.InvariantCultureIgnoreCase).Any();
            return result;
        }
    }
}
