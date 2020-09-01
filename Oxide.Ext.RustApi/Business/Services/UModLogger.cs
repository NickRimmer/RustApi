using System;
using System.IO;
using Oxide.Core;
using Oxide.Ext.RustApi.Primitives.Enums;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class UModLogger<T> : ILogger<T>
    {
        private readonly MinimumLogLevel _logLevel;
        private readonly bool _logToFile;
        private const string LogName = "RustApi";

        public UModLogger(RustApiOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            _logLevel = options.LogLevel;
            _logToFile = options.LogToFile;
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            if ((byte)_logLevel < 4) return;

            var text = $"[{typeof(T).Name}] {message}";
            RustApiExtension.OxideHelper.LogDebug(text);

            LogToFile($"[DEBUG] {text}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            if ((byte)_logLevel < 1) return;

            var text = $"[{typeof(T).Name}] {message}";
            RustApiExtension.OxideHelper.LogError(text);

            LogToFile($"[ERROR] {text}");
        }

        /// <inheritdoc />
        public void Error(Exception ex, string message = null)
        {
            if ((byte)_logLevel < 1) return;

            var finalMessage = string.IsNullOrEmpty(message) ? ex.Message : message;
            var text = $"[{typeof(T).Name}] {finalMessage}";
            RustApiExtension.OxideHelper.LogException(text, ex);

            LogToFile($"[ERROR] {text}");
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            if ((byte)_logLevel < 2) return;

            var text = $"[{typeof(T).Name}] {message}";
            RustApiExtension.OxideHelper.LogWarning(text);

            LogToFile($"[WARN] {text}");
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            if ((byte)_logLevel < 3) return;

            var text = $"[{typeof(T).Name}] {message}";
            RustApiExtension.OxideHelper.LogInfo(text);

            LogToFile($"[INFO] {text}");
        }

        /// <summary>
        /// Lot to file (base on uMod plugin LogToFile method)
        /// </summary>
        /// <param name="text">Text to store to file</param>
        private void LogToFile(string text)
        {
            if (!_logToFile) return;

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
