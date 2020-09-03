using System.Net;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Authentication service.
    /// </summary>
    internal interface IAuthenticationService
    {
        /// <summary>
        /// Get user for request.
        /// </summary>
        /// <param name="context">Http request context.</param>
        /// <param name="userInfo">Found user info.</param>
        /// <returns></returns>
        bool TryToGetUser(HttpListenerContext context, out ApiUserInfo userInfo);

        /// <summary>
        /// Get user by name.
        /// </summary>
        /// <param name="name">User name.</param>
        /// <param name="userInfo">Found user info.</param>
        /// <returns></returns>
        bool TryToGetUser(string name, out ApiUserInfo userInfo);

        /// <summary>
        /// Add a new user.
        /// </summary>
        /// <param name="name">User name (or steamId).</param>
        /// <param name="secret">User secret (or sessionId).</param>
        /// <param name="permissions">List of user permissions.</param>
        /// <returns></returns>
        ApiUserInfo AddUser(string name, string secret, params string[] permissions);
    }
}
