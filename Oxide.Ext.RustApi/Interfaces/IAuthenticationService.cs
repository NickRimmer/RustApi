using System.Net;
using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Authentication service.
    /// </summary>
    internal interface IAuthenticationService
    {
        /// <summary>
        /// Test if sign is valid.
        /// </summary>
        /// <param name="context">Http request context.</param>
        /// <param name="userInfo">Found user info.</param>
        /// <returns></returns>
        bool TryToGetUser(HttpListenerContext context, out ApiUserInfo userInfo);
    }
}
