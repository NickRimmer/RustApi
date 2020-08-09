using Oxide.Core.Plugins;
using System;
using Oxide.Ext.RustApi.Business.Common;

namespace Oxide.Ext.RustApi.Plugins
{
    /// <summary>
    /// Responsible for loading rust API plugins
    /// </summary>
    public class RustApiPluginLoader : PluginLoader
    {
        public override Type[] CorePlugins => new[] { typeof(RustApiPlugin) };
    }
}
