using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Models
{
    /// <summary>
    /// User access configuration.
    /// </summary>
    public class ApiUserInfo
    {
        public ApiUserInfo(string name, string secret, List<string> permissions)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

            Name = name ?? "Unnamed";
            Secret = secret;
            Permissions = permissions ?? new List<string>();
        }

        /// <summary>
        /// User name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Secret word to build sign.
        /// </summary>
        public string Secret { get; }

        /// <summary>
        /// List of permissions
        /// </summary>
        public List<string> Permissions { get; }
    }
}
