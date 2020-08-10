using System.Reflection;
using Oxide.Ext.RustApi.Primitives.Attributes;

namespace Oxide.Ext.RustApi.Primitives.Models
{
    /// <summary>
    /// Api method information.
    /// </summary>
    internal class CommandMethodInfo
    {
        /// <summary>
        /// Method info.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Attribute data.
        /// </summary>
        public ApiCommandAttribute ApiInfo { get; set; }
    }
}
