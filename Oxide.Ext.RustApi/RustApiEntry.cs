﻿using System.Reflection;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Ext.RustApi.Tools;

namespace Oxide.Ext.RustApi
{
    public class RustApiEntry : Extension
    {
        /// <inheritdoc />
        public RustApiEntry(ExtensionManager manager) : base(manager)
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
