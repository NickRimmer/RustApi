namespace Oxide.Ext.RustApi.Primitives.Enums
{
    /// <summary>
    /// Minimum log level configuration values.
    /// </summary>
    public enum MinimumLogLevel: byte
    {
        /// <summary>
        /// Disable all RustApi logs.
        /// </summary>
        Disable = 0,

        /// <summary>
        /// Show only errors messages.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Show errors and warnings messages.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Show errors, warnings and information messages.
        /// </summary>
        Information = 3,

        /// <summary>
        /// Show errors, warnings, information and debug messages (show all).
        /// </summary>
        Debug = 4
    }
}
