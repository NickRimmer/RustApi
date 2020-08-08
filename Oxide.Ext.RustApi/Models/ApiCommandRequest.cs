using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Models
{
    /// <summary>
    /// Request data for command.
    /// </summary>
    public class ApiCommandRequest
    {
        public ApiCommandRequest(string commandName, Dictionary<string, object> parameters)
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Command name.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Command parameters.
        /// </summary>
        public Dictionary<string, object> Parameters { get; }
    }
}
