﻿using Oxide.Ext.RustApi.Models;

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
        /// <param name="sign">Current sign in request.</param>
        /// <param name="route">Route url value.</param>
        /// <param name="requestContent">Content data.</param>
        /// <param name="userInfo">Found user info.</param>
        /// <returns></returns>
        bool TryToGetUser(string user, string sign, string route, string requestContent, out ApiUserInfo userInfo);
    }
}
