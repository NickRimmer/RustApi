using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Steam connection methods.
    /// </summary>
    public interface ISteamConnection
    {
        /// <summary>
        /// Build steam login url with callback url parameter.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callbackUri"></param>
        /// <returns></returns>
        string GetLoginUrl(HttpListenerContext context, Uri callbackUri);

        /// <summary>
        /// Read steam ID by response from steam login page.
        /// </summary>
        /// <param name="steamResponse"></param>
        /// <returns></returns>
        string GetSteamId(Dictionary<string, string> steamResponse);
    }
}
