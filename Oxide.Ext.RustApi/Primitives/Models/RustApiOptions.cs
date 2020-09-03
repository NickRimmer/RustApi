using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Ext.RustApi.Primitives.Enums;

namespace Oxide.Ext.RustApi.Primitives.Models
{
    /// <summary>
    /// Api server options.
    /// </summary>
    internal class RustApiOptions
    {
        public RustApiOptions(
            string endpoint, 
            bool logToFile = false,
            MinimumLogLevel logLevel = MinimumLogLevel.Information
        )
        {
            if (!Enum.IsDefined(typeof(MinimumLogLevel), logLevel)) 
                throw new InvalidEnumArgumentException(nameof(logLevel), (int) logLevel, typeof(MinimumLogLevel));

            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            LogToFile = logToFile;
            LogLevel = logLevel;
        }

        /// <summary>
        /// Endpoint string. (e.g. http://localhost:6667).
        /// </summary>
        public string Endpoint { get; }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public MinimumLogLevel LogLevel { get; }
    }
}
