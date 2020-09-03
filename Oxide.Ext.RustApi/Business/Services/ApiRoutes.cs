using Newtonsoft.Json;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class ApiRoutes : IApiRoutes
    {
        public const string PublicRoutesPrefix = "public";

        private readonly Dictionary<string, ApiRouteHandler<string>> _routes;

        public ApiRoutes()
        {
            _routes = new Dictionary<string, ApiRouteHandler<string>>();
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandler<TRequest> handler) where TRequest : class
            => AddGeneralRoute(route, handler);

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, ApiRouteHandler<object> handler)
            => AddRoute<object>(route, handler);

        /// <inheritdoc />
        public IApiRoutes AddRoute<TRequest>(string route, ApiRouteHandlerNoResponse<TRequest> handler) where TRequest : class =>
            AddGeneralRoute<TRequest>(route, args =>
            {
                handler.Invoke(args);
                return default;
            });

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, ApiRouteHandlerNoResponse<object> handler)
            => AddRoute<object>(route, handler);

        /// <inheritdoc />
        public bool TryGetHandler(string route, out ApiRouteHandler<string> handler)
        {
            var formattedRoute = ApiServer.FormatUrl(route);
            var result = _routes.TryGetValue(formattedRoute, out handler);

            return result;
        }

        /// <summary>
        /// Common logic to register route.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="handler">Request handler.</param>
        /// <returns></returns>
        private IApiRoutes AddGeneralRoute<TRequest>(string route, ApiRouteHandler<TRequest> handler) where TRequest : class
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(route, args => handler.Invoke(BuildTypedArgs<TRequest>(args)));
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
    }
}
