using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using System.Reflection;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.RustApi.Plugins;

namespace Oxide.Ext.RustApi
{
    /// <summary>
    /// General entry point for UMod extension.
    /// </summary>
    public class RustApiExtension : Extension
    {
        private readonly ILogger<RustApiExtension> _logger;

        /// <inheritdoc />
        public RustApiExtension(ExtensionManager manager) : base(manager)
        {
            if (manager == null) return;

            Container = new MicroContainer()
                .AddSingle(this)
                .AddRustApiServices();

            _logger = Container.Get<ILogger<RustApiExtension>>();
        }

        internal MicroContainer Container { get; }

        /// <inheritdoc />
        public override string Name => AssemblyInfo.ReadAssemblyAttribute<AssemblyProductAttribute>().Product;

        /// <inheritdoc />
        public override string Author => AssemblyInfo.ReadAssemblyAttribute<AssemblyCompanyAttribute>().Company;

        /// <inheritdoc />
        public override VersionNumber Version => AssemblyInfo.ReadAssemblyVersion();

        /// <inheritdoc />
        public override void Load()
        {
            Manager.RegisterPluginLoader(new RustApiPluginLoader());
        }

        /// <inheritdoc />
        public override void OnModLoad()
        {
            Container.Get<IApiServer>().Start();

            // update command methods cache
            Interface.uMod.RootPluginManager.OnPluginAdded += OnPluginsUpdate;
            Interface.uMod.RootPluginManager.OnPluginRemoved += OnPluginsUpdate;

            //Interface.uMod.RootPluginManager.
            //Interface.uMod.ServerConsole.Input += s => _logger.Info(s);

            _logger.Info($"{Name} extension loaded");
        }

        /// <inheritdoc />
        public override void OnShutdown()
        {
            Container.Get<IApiServer>()?.Destroy();
            _logger.Info($"{Name} extension unloaded");
        }

        /// <summary>
        /// Reload configuration file
        /// </summary>
        public void ReloadConfiguration()
        {
            // read options from file again
            Container.AddOptions();

            _logger.Info("Extenstion configuration reloaded");
        }

        /// <summary>
        /// Reload plugin commands on plugins updates
        /// </summary>
        /// <param name="plugin"></param>
        private void OnPluginsUpdate(Plugin plugin)
        {
            Container.Get<ICommandRoute>().UpdateApiPluginsCache();
        }
    }
}
