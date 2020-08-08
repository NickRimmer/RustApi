using Oxide.Core;
using Oxide.Ext.RustApi.Interfaces;
using System;
using System.IO;
using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    internal class UModLogger<T> : ILogger<T>
    {
        private readonly RustApiOptions _options;
        private const string LogName = "RustApi";

        public UModLogger(RustApiOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            var text = $"[{typeof(T).Name}] {message}";
            Interface.uMod.LogDebug(text);

            LogToFile($"[DEBUG] {text}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            var text = $"[{typeof(T).Name}] {message}";
            Interface.uMod.LogError(text);

            LogToFile($"[ERROR] {text}");
        }

        /// <inheritdoc />
        public void Error(Exception ex, string message = null)
        {
            var finalMessage = string.IsNullOrEmpty(message) ? ex.Message : message;
            var text = $"[{typeof(T).Name}] {finalMessage}";
            Interface.uMod.LogException(text, ex);

            LogToFile($"[ERROR] {text}");
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            var text = $"[{typeof(T).Name}] {message}";
            Interface.uMod.LogWarning(text);

            LogToFile($"[WARN] {text}");
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            var text = $"[{typeof(T).Name}] {message}";
            Interface.uMod.LogInfo(text);

            LogToFile($"[INFO] {text}");
        }

        /// <summary>
        /// Lot to file (base on uMod plugin LogToFile method)
        /// </summary>
        /// <param name="text">Text to store to file</param>
        private void LogToFile(string text)
        {
            if (!_options.LogToFile) return;

            var now = DateTime.Now;
            var path = Path.Combine(Interface.Oxide.LogDirectory, LogName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var targetFileName = $"{LogName.ToLower()}_{now:yyyy-MM-dd}.txt";
            var targetPath = Path.Combine(path, Utility.CleanPath(targetFileName));
            
            using (var writer = new StreamWriter(targetPath, true)) 
                writer.WriteLine($"{now:HH:mm:ss} {text}");
        }
    }
}
