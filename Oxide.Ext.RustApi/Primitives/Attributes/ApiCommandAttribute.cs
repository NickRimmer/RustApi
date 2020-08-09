using System;

namespace Oxide.Ext.RustApi.Primitives.Attributes
{
    /// <summary>
    /// ApiCommand attribute for Plugins.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class ApiCommandAttribute: Attribute
    {
        /// <summary>
        /// Command name.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// List of required permissions.
        /// </summary>
        public string[] RequiredPermissions { get; }

        /// <summary>
        /// ApiCommand attribute for plugin methods.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="requiredPermissions">Required permissions.</param>
        public ApiCommandAttribute(string commandName, params string[] requiredPermissions)
        {
            if (string.IsNullOrWhiteSpace(commandName)) throw new ArgumentException(nameof(commandName));

            CommandName = commandName;
            RequiredPermissions = requiredPermissions ?? throw new ArgumentNullException(nameof(requiredPermissions));
        }
    }
}
