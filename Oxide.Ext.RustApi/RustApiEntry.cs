using System.Reflection;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Services;
using Oxide.Ext.RustApi.Tools;

namespace Oxide.Ext.RustApi
{
    /// <summary>
    /// General entry point for UMod extension.
    /// </summary>
    public class RustApiEntry : Extension
    {
        private MicroContainer _services;

        /// <inheritdoc />
        public RustApiEntry(ExtensionManager manager) : base(manager)
        {
            _services = new MicroContainer()
                .Add<ILogger, UModLogger>();
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
            _services.Get<ILogger>().Info($"{Name} extension loaded");
            base.OnModLoad();
        }

        /// <inheritdoc />
        public override void OnShutdown()
        {
            _services.Get<ILogger>().Info($"{Name} extension unloaded");
            base.OnShutdown();
        }
    }
}
