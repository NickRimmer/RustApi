using Oxide.Core;
using Oxide.Ext.RustApi.Attributes;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using Oxide.Ext.RustApi.Routes;

namespace Oxide.Ext.RustApi
{
    internal static class RustApiRoutes
    {
        public static MicroContainer AddRustApiRoutes(this MicroContainer container)
        {
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes
                .AddRoute<ApiHookRequest>("hook", HookRoute.OnCallHook)
                .AddRoute<ApiCommandRequest>("command", (user, request) => container.Get<ICommandRoute>().OnCallCommand(user, request));

            return container;
        }

        /// <summary>
        /// Validate user permissions.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="requiredPermissions">Required permission.</param>
        public static bool IsUserHasAccess(ApiUserInfo user, params string[] requiredPermissions)
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
