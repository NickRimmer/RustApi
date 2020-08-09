using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;

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
    }

    internal static class SystemRouteExtension
    {
        public static MicroContainer AddSystemRoutes(this MicroContainer container)
        {
            container.AddSingle<ICommandRoute, CommandRoute>();
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes
                .AddRoute<string>("system/ping", (user, content) => container.Get<ISystemRoute>().OnPing())
                .AddRoute("system/test/debug", (user) => container.Get<ISystemRoute>().OnTestDebug())
                .AddRoute("system/test/info", (user) => container.Get<ISystemRoute>().OnTestInfo())
                .AddRoute("system/test/warning", (user) => container.Get<ISystemRoute>().OnTestWarning())
                .AddRoute("system/test/error", (user) => container.Get<ISystemRoute>().OnTestError());

            return container;
        }
    }
}
