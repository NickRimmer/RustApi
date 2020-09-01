using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Oxide.Core;

[assembly: InternalsVisibleTo("Oxide.Ext.RustApi.Tests.Unit")]
[assembly: InternalsVisibleTo("Oxide.Ext.RustApi.Tests.FakeServer")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Oxide.Ext.RustApi.Business.Common
{
    internal static class AssemblyInfo
    {
        /// <summary>
        /// Short way to read assembly attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static T ReadAssemblyAttribute<T>() where T : Attribute => (T) typeof(RustApiExtension).Assembly.GetCustomAttribute(typeof(T));

        /// <summary>
        /// Build extension version from assembly version
        /// </summary>
        /// <returns></returns>
        internal static VersionNumber ReadAssemblyVersion()
        {
            var assemblyInfo = typeof(RustApiExtension).Assembly.GetName();
            var version = assemblyInfo.Version;

            var result = new VersionNumber(version.Major, version.Minor, version.Build);
            return result;
        }
    }
}
