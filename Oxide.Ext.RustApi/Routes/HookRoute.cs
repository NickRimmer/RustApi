using System.Security;
using Oxide.Core;
using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Routes
{
    internal static class HookRoute
    {
        /// <summary>
        /// On Hook execute API request.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="apiHookInfo">Hook request information.</param>
        public static object OnCallHook(ApiUserInfo user, ApiHookRequest apiHookInfo)
        {
            const string hooksAccessPermission = "hooks";
            
            if(!RustApiRoutes.IsUserHasAccess(user, hooksAccessPermission))
                throw new SecurityException($"User '{user}' hasn't required permission '{hooksAccessPermission}'");

            var result = Interface.uMod.CallHook(apiHookInfo.HookName, apiHookInfo.Parameters);
            return result;
        }
    }
}
