using System.Reflection;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Ext.RustApi.Tools;

namespace Oxide.Ext.RustApi
{
    /// <summary>
    /// Base RustApi class
    /// </summary>
    public abstract class RustApiBase : Extension
    {
        /// <inheritdoc />
        protected RustApiBase(ExtensionManager manager) : base(manager)
        {

        }

        /// <inheritdoc />
        public override string Name => AssemblyInfo.ReadAssemblyAttribute<AssemblyProductAttribute>().Product;

        /// <inheritdoc />
        public override string Author => AssemblyInfo.ReadAssemblyAttribute<AssemblyCompanyAttribute>().Company;

        /// <inheritdoc />
        public override VersionNumber Version => AssemblyInfo.ReadAssemblyVersion();
    }
}
