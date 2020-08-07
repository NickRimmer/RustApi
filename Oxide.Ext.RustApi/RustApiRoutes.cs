using Oxide.Core;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models;
using Oxide.Ext.RustApi.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Oxide.Ext.RustApi
{
    internal static class RustApiRoutes
    {
        public static MicroContainer AddRustApiRoutes(this MicroContainer container)
        {
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes
                .AddRoute<HookRequestModel>("hook", OnCallHook);

            return container;
        }

        /// <summary>
        /// Validate user permissions.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="requiredPermission">Required permission.</param>
        private static void ValidateUserPermission(ApiUserInfo user, string requiredPermission)
        {
            // admin permission to allow everything
            const string adminAccessPermission = "admin";
            if (user.Permissions.Any(x => x.Equals(adminAccessPermission, StringComparison.InvariantCultureIgnoreCase))) return;

            // try to find required permissions
            if (!user.Permissions.Any(x => x.Equals(requiredPermission, StringComparison.InvariantCultureIgnoreCase)))
                throw new SecurityException($"User '{user}' hasn't required permission '{requiredPermission}'");
        }

        /// <summary>
        /// On Hook execute API request.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="hookInfo">Hook request information.</param>
        private static object OnCallHook(ApiUserInfo user, HookRequestModel hookInfo)
        {
            const string hooksAccessPermission = "hook";
            ValidateUserPermission(user, hooksAccessPermission);

            var result = Interface.uMod.CallHook(hookInfo.HookName, hookInfo.Parameters);
            return result;
        }
    }
}
