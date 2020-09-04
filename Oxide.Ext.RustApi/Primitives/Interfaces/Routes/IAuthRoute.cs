using System;
using System.Net;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Auth methods.
    /// </summary>
    internal interface IAuthRoute
    {
        /// <summary>
        /// Get url for login page.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Uri Login(HttpListenerContext context);

        /// <summary>
        /// Validate login response and return session id.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Uri Confirm(HttpListenerContext context);
    }
}
