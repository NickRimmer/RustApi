using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Primitives.Models
{
    /// <summary>
    /// Request data for hook execution
    /// </summary>
    public class ApiHookRequest
    {
        public ApiHookRequest(string hookName, Dictionary<string, object> parameters)
        {
            HookName = hookName ?? throw new ArgumentNullException(nameof(hookName));
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Hook name
        /// </summary>
        public string HookName { get; }

        /// <summary>
        /// Hook parameters
        /// </summary>
        public Dictionary<string, object> Parameters { get; }
    }
}
