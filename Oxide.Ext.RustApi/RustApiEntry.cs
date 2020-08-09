using System;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Ext.RustApi.Services;
using System.Reflection;
using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;

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
            if (manager == null) return;
            
            _services = new MicroContainer().AddRustApiServices();
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
            _services.Get<IApiServer>().Start();

            // update command methods cache
            Interface.uMod.RootPluginManager.OnPluginAdded += OnPluginsUpdate;
            Interface.uMod.RootPluginManager.OnPluginRemoved += OnPluginsUpdate;

            _logger.Info($"{Name} extension loaded");
        }

        private void OnPluginsUpdate(Plugin plugin)
        {
            _services.Get<ICommandRoute>().UpdateApiPluginsCache();
        }

        /// <inheritdoc />
        public override void OnShutdown()
        {
            _services.Get<IApiServer>()?.Destroy();
            _logger.Info($"{Name} extension unloaded");
        }
    }
}
