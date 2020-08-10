using System.Security;
using Oxide.Core;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Routes
{
    /// <inheritdoc cref="IHookRoute"/>
    internal class HookRoute : RouteBase, IHookRoute
    {

        /// <inheritdoc cref="IHookRoute"/>
        public object OnCallHook(ApiUserInfo user, ApiHookRequest apiHookInfo)
        {
            const string hooksAccessPermission = "hooks";

            if (!IsUserHasAccess(user, hooksAccessPermission))
                throw new SecurityException($"User '{user}' hasn't required permission '{hooksAccessPermission}'");

            var result = Interface.uMod.CallHook(apiHookInfo.HookName, apiHookInfo.Parameters);
            return result;
        }
    }

    internal static class HookRouteExtension
    {
        public static MicroContainer AddHookRoutes(this MicroContainer container)
        {
            container.AddSingle<IHookRoute, HookRoute>();
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes.AddRoute<ApiHookRequest>(
                "hook",
                (user, request) => container.Get<IHookRoute>().OnCallHook(user, request));

            return container;
        }
    }
}
