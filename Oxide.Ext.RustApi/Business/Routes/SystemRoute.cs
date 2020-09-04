using Oxide.Ext.RustApi.Business.Common;
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
                .AddRoute("system/ping", args => container.Get<ISystemRoute>().OnPing(), true)
                .AddRoute("system/test/debug", args => container.Get<ISystemRoute>().OnTestDebug(), true)
                .AddRoute("system/test/info", args => container.Get<ISystemRoute>().OnTestInfo(), true)
                .AddRoute("system/test/warning", args => container.Get<ISystemRoute>().OnTestWarning(), true)
                .AddRoute("system/test/error", args => container.Get<ISystemRoute>().OnTestError(), true)
                .AddRoute("system/user", args => container.Get<ISystemRoute>().OnUserInfo(args.User));

            return container;
        }
    }
}
