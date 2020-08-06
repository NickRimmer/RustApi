using System.Reflection;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models.Options;
using Oxide.Ext.RustApi.Services;
using Oxide.Ext.RustApi.Tools;

namespace Oxide.Ext.RustApi
{
    /// <summary>
    /// General entry point for UMod extension.
    /// </summary>
    public class RustApiEntry : Extension
    {
        private readonly MicroContainer _services;
        private readonly ILogger<RustApiEntry> _logger;

        /// <inheritdoc />
        public RustApiEntry(ExtensionManager manager) : base(manager)
        {
            _services = BuildServices();
            _logger = _services.Get<ILogger<RustApiEntry>>();
        }

        /// <inheritdoc />
        public override string Name => AssemblyInfo.ReadAssemblyAttribute<AssemblyProductAttribute>().Product;

        /// <inheritdoc />
        public override string Author => AssemblyInfo.ReadAssemblyAttribute<AssemblyCompanyAttribute>().Company;

        /// <inheritdoc />
        public override VersionNumber Version => AssemblyInfo.ReadAssemblyVersion();

        /// <inheritdoc />
        public override void OnModLoad()
        {
            _services.Get<ApiServer>().Start();
            _logger.Info($"{Name} extension loaded");
        }

        /// <inheritdoc />
        public override void OnShutdown()
        {
            _services.Get<ApiServer>()?.Dispose();
            _logger.Info($"{Name} extension unloaded");
        }

        /// <summary>
        /// Build micro container with required services
        /// </summary>
        /// <returns></returns>
        private static MicroContainer BuildServices()
        {
            var result = new MicroContainer()
                .Add(typeof(ILogger<>), typeof(UModLogger<>))
                .AddSingle(new ApiServerOptions { Endpoint = "http://localhost:6667" }) //TODO read from configuration
                .AddSingle<ApiServer>();

            var apiServer = result.Get<ApiServer>();
            ConfigureRoutes(apiServer);

            return result;
        }

        private static void ConfigureRoutes(ApiServer apiServer)
        {
            //TODO specify route handlers
        }
    }
}
