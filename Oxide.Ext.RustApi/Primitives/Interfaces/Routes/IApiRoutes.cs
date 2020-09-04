using System;
using Oxide.Ext.RustApi.Primitives.Models;
using System.Net;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Api server routes collection
    /// </summary>
    internal interface IApiRoutes
    {
        /// <summary>
        /// Route handler.
        /// </summary>
        /// <typeparam name="TRequest">Expected data type of request.</typeparam>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Route request arguments.</param>
        /// <param name="isPublic">Set true to make this route available without user credentials.</param>
        /// <returns></returns>
        IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandler<TRequest> handler, bool isPublic = false) where TRequest : class;

        /// <summary>
        /// Route handler without request body.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Route request arguments.</param>
        /// <param name="isPublic">Set true to make this route available without user credentials.</param>
        /// <returns></returns>
        IApiRoutes AddRoute(string route, ApiRouteHandler<object> handler, bool isPublic = false);

        /// <summary>
        /// Route handler without response.
        /// </summary>
        /// <typeparam name="TRequest">Expected data type of request.</typeparam>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Route request arguments.</param>
        /// <param name="isPublic">Set true to make this route available without user credentials.</param>
        /// <returns></returns>
        IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandlerNoResponse<TRequest> handler, bool isPublic = false) where TRequest : class;

        /// <summary>
        /// Route handler without request body and response.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Route request arguments.</param>
        /// <param name="isPublic">Set true to make this route available without user credentials.</param>
        /// <returns></returns>
        IApiRoutes AddRoute(string route, ApiRouteHandlerNoResponse<object> handler, bool isPublic = false);

        /// <summary>
        /// Try to get handler for route.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Found handler.</param>
        /// <param name="publicOnly">Looking for public routes only (for example in case of anonymous user).</param>
        /// <returns>True if route was found.</returns>
        bool TryGetHandler(string route, out ApiRouteHandler<string> handler, bool publicOnly = false);
    }

    /// <summary>
    /// Api route request arguments.
    /// </summary>
    internal class ApiRouteRequestArgs<TData> where TData: class
    {
        public ApiRouteRequestArgs(ApiUserInfo user, TData data, HttpListenerContext context)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Data = data;
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// User information
        /// </summary>
        public ApiUserInfo User { get; }

        /// <summary>
        /// Request content.
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// Request context.
        /// </summary>
        public HttpListenerContext Context { get; }
    }

    /// <summary>
    /// Route handler with expected request data type.
    /// </summary>
    /// <typeparam name="TRequest">Expected data type of request.</typeparam>
    /// <param name="args">Request arguments.</param>
    /// <returns></returns>
    internal delegate object ApiRouteHandler<TRequest>(ApiRouteRequestArgs<TRequest> args) where TRequest : class;

    /// <summary>
    /// Route handler with expected request data type and without response.
    /// </summary>
    /// <typeparam name="TRequest">Expected data type of request.</typeparam>
    /// <param name="args">Request arguments.</param>
    /// <returns></returns>
    internal delegate void ApiRouteHandlerNoResponse<TRequest>(ApiRouteRequestArgs<TRequest> args) where TRequest : class;
}
