using Newtonsoft.Json;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class ApiRoutes : IApiRoutes
    {
        private readonly Dictionary<string, RouteInfo> _routes;

        public ApiRoutes()
        {
            _routes = new Dictionary<string, RouteInfo>();
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandler<TRequest> handler, bool isPublic = false) where TRequest : class
            => AddGeneralRoute(route, handler, isPublic);

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, ApiRouteHandler<object> handler, bool isPublic = false)
            => AddRoute<object>(route, handler, isPublic);

        /// <inheritdoc />
        public IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandlerNoResponse<TRequest> handler, bool isPublic = false) where TRequest : class =>
            AddGeneralRoute<TRequest>(route, args =>
            {
                handler.Invoke(args);
                return default;
            }, isPublic);

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, ApiRouteHandlerNoResponse<object> handler, bool isPublic = false)
            => AddRoute<object>(route, handler, isPublic);

        /// <inheritdoc />
        public bool TryGetHandler(string route, out ApiRouteHandler<string> handler, bool publicOnly = false)
        {
            handler = default;

            // let's find route with specified name
            var formattedRoute = ApiServer.FormatUrl(route);
            var result = _routes.TryGetValue(formattedRoute, out var handlerInfo);

            // if route not found
            if (!result || handlerInfo == default) return false;

            // if route is not public, but we looking for public handler only
            if (publicOnly && !handlerInfo.IsPublic) return false;

            handler = handlerInfo.Handler;
            return result;
        }

        /// <summary>
        /// Common logic to register route.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Request handler.</param>
        /// <param name="isPublic">>Set true to make this route available without user credentials.</param>
        /// <returns></returns>
        private IApiRoutes AddGeneralRoute<TRequest>(string route, ApiRouteHandler<TRequest> handler, bool isPublic = false) where TRequest : class
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            var routeInfo = new RouteInfo(args => handler.Invoke(BuildTypedArgs<TRequest>(args)), isPublic);
            _routes.Add(route, routeInfo);

            return this;
        }

        /// <summary>
        /// Convert internal route args to expected.
        /// </summary>
        /// <typeparam name="TRequest">Expected type of data.</typeparam>
        /// <param name="args">Internal args.</param>
        /// <returns></returns>
        private static ApiRouteRequestArgs<TRequest> BuildTypedArgs<TRequest>(ApiRouteRequestArgs<string> args) where TRequest : class =>
            new ApiRouteRequestArgs<TRequest>(
                args.User,
                JsonConvert.DeserializeObject<TRequest>(args.Data),
                args.Context);

        /// <summary>
        /// Configured route information.
        /// </summary>
        private class RouteInfo
        {
            public RouteInfo(ApiRouteHandler<string> handler, bool isPublic = false)
            {
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
                IsPublic = isPublic;
            }

            /// <summary>
            /// Route handler.
            /// </summary>
            public ApiRouteHandler<string> Handler { get; }

            /// <summary>
            /// Public route flag.
            /// </summary>
            public bool IsPublic { get; }
        }
    }
}
