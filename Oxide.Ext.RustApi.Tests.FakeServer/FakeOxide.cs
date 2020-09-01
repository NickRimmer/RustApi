using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Primitives.Interfaces;

namespace Oxide.Ext.RustApi.Tests.FakeServer
{
    internal class FakeOxide: IOxideHelper
    {
        /// <inheritdoc />
        public event PluginEvent OnPluginsUpdate;

        /// <inheritdoc />
        public T GetExtension<T>() where T : Extension => throw new Exception("Method not allowed on Fake server");

        /// <inheritdoc />
        public string GetInstanceDirectory() => Directory.GetCurrentDirectory();

        /// <inheritdoc />
        public void LogDebug(string message) => WriteLog("dbg", message);

        /// <inheritdoc />
        public void LogError(string message) => WriteLog("err", message, ConsoleColor.Red);

        /// <inheritdoc />
        public void LogException(string message, Exception ex)
        {
            if (!string.IsNullOrEmpty(message))  WriteLog("err", message, ConsoleColor.Red);
            
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"[err] {ex.Message}\n");
            Console.WriteLine(ex.StackTrace);

            Console.ResetColor();
        }

        /// <inheritdoc />
        public void LogWarning(string message) => WriteLog("wrn", message, ConsoleColor.Yellow);

        /// <inheritdoc />
        public void LogInfo(string message) => WriteLog("inf", message, ConsoleColor.Blue);

        /// <inheritdoc />
        public object CallHook(string hookName, params object[] args) => throw new Exception("Method not allowed on Fake server");

        /// <inheritdoc />
        public IEnumerable<Plugin> GetPlugins() => throw new Exception("Method not allowed on Fake server");

        private void WriteLog(string type, string message, ConsoleColor prefixColor = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = prefixColor;
            Console.Write($"[{type}] ");

            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}
