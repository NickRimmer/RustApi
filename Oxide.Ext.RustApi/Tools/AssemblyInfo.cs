﻿using System;
using System.Reflection;
using Oxide.Core;

namespace Oxide.Ext.RustApi.Tools
{
    internal static class AssemblyInfo
    {
        /// <summary>
        /// Short way to read assembly attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static T ReadAssemblyAttribute<T>() where T : Attribute => (T) typeof(RustApiEntry).Assembly.GetCustomAttribute(typeof(T));

        /// <summary>
        /// Build extension version from assembly version
        /// </summary>
        /// <returns></returns>
        internal static VersionNumber ReadAssemblyVersion()
        {
            var assemblyInfo = typeof(RustApiEntry).Assembly.GetName();
            var version = assemblyInfo.Version;

            var result = new VersionNumber(version.Major, version.Minor, version.Build);
            return result;
        }
    }
}
