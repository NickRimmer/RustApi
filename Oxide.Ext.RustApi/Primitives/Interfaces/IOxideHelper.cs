using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Oxide library using.
    /// </summary>
    internal interface IOxideHelper
    {
        /// <summary>
        /// On plugins load/unload.
        /// </summary>
        event PluginEvent OnPluginsUpdate;

        /// <summary>
        /// Get extension.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetExtension<T>() where T : Extension;

        /// <summary>
        /// Get current instance directory.
        /// </summary>
        /// <returns></returns>
        string GetInstanceDirectory();

        /// <summary>
        /// Log debug message.
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(string message);

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void LogException(string message, Exception ex);

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message);

        /// <summary>
        /// Log information message.
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message);

        /// <summary>
        /// Call hook.
        /// </summary>
        /// <param name="hookName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object CallHook(string hookName, params object[] args);

        /// <summary>
        /// Get list of loaded plugins.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Plugin> GetPlugins();
    }
}
