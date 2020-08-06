using System;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Internal logger.
    /// </summary>
    public interface ILogger<T>
    {
        /// <summary>
        /// Log debug information.
        /// </summary>
        /// <param name="format">Formatted message.</param>
        /// <param name="args">Message arguments.</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Log error information.
        /// </summary>
        /// <param name="format">Formatted message.</param>
        /// <param name="args">Message arguments.</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="ex">Exception instance.</param>
        /// <param name="format">Formatted message.</param>
        /// <param name="args">Message arguments.</param>
        void Error(Exception ex, string format = null, params object[] args);

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="format">Formatted message.</param>
        /// <param name="args">Message arguments.</param>
        void Warning(string format, params object[] args);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="format">Formatted message.</param>
        /// <param name="args">Message arguments.</param>
        void Info(string format, params object[] args);
    }
}
