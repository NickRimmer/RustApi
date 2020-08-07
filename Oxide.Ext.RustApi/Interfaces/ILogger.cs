using System;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Internal logger.
    /// </summary>
    internal interface ILogger<T>
    {
        /// <summary>
        /// Log debug information.
        /// </summary>
        /// <param name="message">Formatted message.</param>
        void Debug(string message);

        /// <summary>
        /// Log error information.
        /// </summary>
        /// <param name="message">Formatted message.</param>
        void Error(string message);

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="ex">Exception instance.</param>
        /// <param name="message">Formatted message.</param>
        void Error(Exception ex, string message = null);

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="message">Formatted message.</param>
        void Warning(string message);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="message">Formatted message.</param>
        void Info(string message);
    }
}
