using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Primitives.Interfaces;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class OxideHelper : IOxideHelper
    {
        public OxideHelper()
        {
            Interface.uMod.RootPluginManager.OnPluginAdded += OnPluginsUpdateHandler;
            Interface.uMod.RootPluginManager.OnPluginRemoved += OnPluginsUpdateHandler;
        }

        /// <inheritdoc />
        public event PluginEvent OnPluginsUpdate;

        /// <inheritdoc />
        public T GetExtension<T>() where T : Extension => (T) Interface
            .uMod
            .GetAllExtensions()
            .First(x => x is T);

        /// <inheritdoc />
        public string GetInstanceDirectory() => Interface.uMod.InstanceDirectory;

        /// <inheritdoc />
        public void LogDebug(string message) => Interface.uMod.LogDebug(message);

        /// <inheritdoc />
        public void LogError(string message) => Interface.uMod.LogError(message);

        /// <inheritdoc />
        public void LogException(string message, Exception ex) => Interface.uMod.LogException(message, ex);

        /// <inheritdoc />
        public void LogWarning(string message) => Interface.uMod.LogWarning(message);

        /// <inheritdoc />
        public void LogInfo(string message) => Interface.uMod.LogInfo(message);

        /// <inheritdoc />
        public object CallHook(string hookName, params object[] args) => Interface.uMod.CallHook(hookName, args);

        /// <inheritdoc />
        public IEnumerable<Plugin> GetPlugins() => Interface.uMod.RootPluginManager.GetPlugins();

        /// <summary>
        /// Event handler on plugins updates.
        /// </summary>
        /// <param name="plugin">Updated plugin instance.</param>
        private void OnPluginsUpdateHandler(Plugin plugin)
        {
            if (OnPluginsUpdate == default) LogWarning("No one subscribed to event update events");
            else OnPluginsUpdate.Invoke(plugin);
        }
    }
}
