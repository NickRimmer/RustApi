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
                .AddRoute<ApiHookRequest>("hook", OnCallHook);

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
        /// <param name="apiHookInfo">Hook request information.</param>
        private static object OnCallHook(ApiUserInfo user, ApiHookRequest apiHookInfo)
        {
            const string hooksAccessPermission = "hook";
            ValidateUserPermission(user, hooksAccessPermission);

            var result = Interface.uMod.CallHook(apiHookInfo.HookName, apiHookInfo.Parameters);
            return result;
        }
    }
}
