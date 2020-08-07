using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Models.Options
{
    /// <summary>
    /// Api server options.
    /// </summary>
    internal class RustApiOptions
    {
        public RustApiOptions(string endpoint, List<ApiUserInfo> users = null)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
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
    }
}
