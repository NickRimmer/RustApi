using Oxide.Ext.RustApi.Models.Options;
using System;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Api server routes collection
    /// </summary>
    public interface IApiRoutes
    {
        /// <summary>
        /// Add route with expected response data model.
        /// </summary>
        /// <typeparam name="T">Expected response data model.</typeparam>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        IApiRoutes AddRoute<T>(string route, Func<UserOptions, T, object> callback) where T : class;

        /// <summary>
        /// Add simple route.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        IApiRoutes AddRoute(string route, Func<UserOptions, object> callback);

        /// <summary>
        /// Add simple route without response and request data.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        IApiRoutes AddRoute(string route, Action<UserOptions> callback);

        /// <summary>
        /// Add simple route without response data.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        IApiRoutes AddRoute<T>(string route, Action<UserOptions, T> callback);

        /// <summary>
        /// Try to get handler for route
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        bool TryGetHandler(string route, out RouteHandlerArgs handler);
    }

    /// <summary>
    /// Route handler delegate
    /// </summary>
    /// <param name="userInfo">User information</param>
    /// <param name="content">Request content</param>
    /// <returns></returns>
    public delegate object RouteHandlerArgs(UserOptions userInfo, string content);
}
