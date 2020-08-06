using Oxide.Core;
using Oxide.Ext.RustApi.Interfaces;
using System;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    public class UModLogger : ILogger
    {
        /// <inheritdoc />
        public void Debug(string format, params object[] args)
        {
            Interface.uMod.LogDebug(format, args);
        }

        /// <inheritdoc />
        public void Error(string format, params object[] args)
        {
            Interface.uMod.LogError(format, args);
        }

        /// <inheritdoc />
        public void Error(Exception ex, string format = null, params object[] args)
        {
            var message = string.IsNullOrEmpty(format) ? ex.Message : string.Format(format, args);
            Interface.uMod.LogException(message, ex);
        }

        /// <inheritdoc />
        public void Warning(string format, params object[] args)
        {
            Interface.uMod.LogWarning(format, args);
        }

        /// <inheritdoc />
        public void Info(string format, params object[] args)
        {
            Interface.uMod.LogInfo(format, args);
        }
    }
}
