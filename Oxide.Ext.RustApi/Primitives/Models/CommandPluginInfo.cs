using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Business.Routes;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Primitives.Models
{
    /// <summary>
    /// Api plugin information.
    /// </summary>
    internal class CommandPluginInfo
    {
        /// <summary>
        /// Plugin instance.
        /// </summary>
        public Plugin Instance { get; set; }

        /// <summary>
        /// Plugin methods.
        /// </summary>
        public IReadOnlyList<CommandMethodInfo> Methods { get; set; }
    }
}
