﻿using Newtonsoft.Json;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models.Options;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    internal class ApiRoutes : IApiRoutes
    {
        private readonly Dictionary<string, RouteHandlerArgs> _routes;

        public ApiRoutes()
        {
            _routes = new Dictionary<string, RouteHandlerArgs>();
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute<T>(string route, Func<UserOptions, T, object> callback) where T : class
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (user, response) =>
            {
                T data = string.IsNullOrEmpty(response)
                    ? default
                    : JsonConvert.DeserializeObject<T>(response);

                return callback.Invoke(user, data);
            });

            return this;
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, Func<UserOptions, object> callback)
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (user, _) => callback.Invoke(user));
            return this;
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute(string route, Action<UserOptions> callback)
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (user, _) =>
            {
                callback.Invoke(user);
                return default;
            });

            return this;
        }

        /// <inheritdoc />
        public IApiRoutes AddRoute<T>(string route, Action<UserOptions, T> callback)
        {
            var url = ApiServer.FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (user, response) =>
            {
                T data = string.IsNullOrEmpty(response)
                    ? default
                    : JsonConvert.DeserializeObject<T>(response);

                callback.Invoke(user, data);
                return default;
            });

            return this;
        }

        /// <inheritdoc />
        public bool TryGetHandler(string route, out RouteHandlerArgs handler)
        {
            var formattedRoute = ApiServer.FormatUrl(route);
            var result = _routes.TryGetValue(formattedRoute, out handler);

            return result;
        }
    }
}
