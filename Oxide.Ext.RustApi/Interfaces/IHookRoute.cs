using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Route for hooks execution
    /// </summary>
    public interface IHookRoute
    {
        /// <summary>
        /// On Hook execute API request.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <param name="apiHookInfo">Hook request information.</param>
        object OnCallHook(ApiUserInfo user, ApiHookRequest apiHookInfo);
    }
}
