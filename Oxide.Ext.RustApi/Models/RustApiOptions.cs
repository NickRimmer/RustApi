using System;
using System.Collections.Generic;
using System.ComponentModel;
using Oxide.Ext.RustApi.Enums;

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
            bool logToFile = false,
            MinimumLogLevel logLevel = MinimumLogLevel.Error
        )
        {
            if (!Enum.IsDefined(typeof(MinimumLogLevel), logLevel)) 
                throw new InvalidEnumArgumentException(nameof(logLevel), (int) logLevel, typeof(MinimumLogLevel));

            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            LogToFile = logToFile;
            Users = users ?? new List<ApiUserInfo>();
            LogLevel = logLevel;
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

        /// <summary>
        /// Log level
        /// Disable - Disable (no logs)
        /// Error - Errors only (default value)
        /// Warning - Warnings and previous
        /// Information - Info and previous
        /// Debug - Debug and previous (all logs)
        /// </summary>
        public MinimumLogLevel LogLevel { get; }
    }
}
