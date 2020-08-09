using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Models
{
    /// <summary>
    /// Api server options.
    /// </summary>
    internal class RustApiOptions
    {
        public RustApiOptions(
            string endpoint, 
            List<ApiUserInfo> users = null,
            bool logToFile = false
        )
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            LogToFile = logToFile;
            Users = users ?? new List<ApiUserInfo>();
        }

        /// <summary>
        /// Endpoint string. (e.g. http://localhost:6667).
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// List of users
        /// </summary>
        public List<ApiUserInfo> Users { get; }

        /// <summary>
        /// Enable logs storing into file
        /// </summary>
        public bool LogToFile { get; }
    }
}
