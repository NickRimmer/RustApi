using Oxide.Core;
using Oxide.Ext.RustApi.Interfaces;
using System;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    public class UModLogger<T> : ILogger<T>
    {
        /// <inheritdoc />
        public void Debug(string message)
        {
            Interface.uMod.LogDebug($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            Interface.uMod.LogError($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Error(Exception ex, string message = null)
        {
            var finalMessage = string.IsNullOrEmpty(message) ? ex.Message : message;
            Interface.uMod.LogException($"[{typeof(T).Name}] {finalMessage}", ex);
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            Interface.uMod.LogWarning($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            Interface.uMod.LogInfo($"[{typeof(T).Name}] {message}");
        }
    }
}
