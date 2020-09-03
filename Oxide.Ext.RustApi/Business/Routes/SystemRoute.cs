﻿using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Routes
{
    /// <inheritdoc />
    internal class SystemRoute : ISystemRoute
    {
        private readonly ILogger<SystemRoute> _logger;

        public SystemRoute(ILogger<SystemRoute> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public string OnPing() => "Pong";

        /// <inheritdoc />
        public void OnTestDebug() => _logger.Debug("Debug test log");

        /// <inheritdoc />
        public void OnTestInfo() => _logger.Info("Info test log");

        /// <inheritdoc />
        public void OnTestWarning() => _logger.Warning("Warning test log");

        /// <inheritdoc />
        public void OnTestError() => _logger.Error("Error test log");

        /// <inheritdoc />
        public object OnUserInfo(ApiUserInfo user) => new
        {
            user.Name,
            user.Permissions
        };
    }

    internal static class SystemRouteExtension
    {
        public static MicroContainer AddSystemRoutes(this MicroContainer container)
        {
            container.AddSingle<ICommandRoute, CommandRoute>();
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes
                .AddRoute($"{ApiRoutes.PublicRoutesPrefix}/system/ping", args => container.Get<ISystemRoute>().OnPing())
                .AddRoute($"{ApiRoutes.PublicRoutesPrefix}/system/test/debug", args => container.Get<ISystemRoute>().OnTestDebug())
                .AddRoute($"{ApiRoutes.PublicRoutesPrefix}/system/test/info", args => container.Get<ISystemRoute>().OnTestInfo())
                .AddRoute($"{ApiRoutes.PublicRoutesPrefix}/system/test/warning", args => container.Get<ISystemRoute>().OnTestWarning())
                .AddRoute($"{ApiRoutes.PublicRoutesPrefix}/system/test/error", args => container.Get<ISystemRoute>().OnTestError())
                .AddRoute("system/user", args => container.Get<ISystemRoute>().OnUserInfo(args.User));

            return container;
        }
    }
}
