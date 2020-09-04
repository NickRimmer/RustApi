using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Primitives.Models
{
    /// <summary>
    /// User access configuration.
    /// </summary>
    public class ApiUserInfo
    {
        public ApiUserInfo(string name, string secret, List<string> permissions)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Secret = secret;
            Permissions = permissions ?? new List<string>();

            IsAnonymous = false;
        }

        /// <summary>
        /// Create anonymous info
        /// </summary>
        private ApiUserInfo()
        {
            Name = string.Empty;
            Secret = string.Empty;
            Permissions = new List<string>();

            IsAnonymous = true;
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

        /// <summary>
        /// Anonymous flag
        /// </summary>
        [JsonIgnore]
        public bool IsAnonymous { get; }

        /// <summary>
        /// Get anonymous user info
        /// </summary>
        public static ApiUserInfo Anonymous => new ApiUserInfo();
    }
}
