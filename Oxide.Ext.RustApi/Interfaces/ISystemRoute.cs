using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// System and test routes.
    /// </summary>
    public interface ISystemRoute
    {
        /// <summary>
        /// Ping method.
        /// </summary>
        /// <returns></returns>
        string OnPing();

        /// <summary>
        /// To test logger.
        /// </summary>
        void OnTestDebug();

        /// <summary>
        /// To test logger.
        /// </summary>
        void OnTestInfo();

        /// <summary>
        /// To test logger.
        /// </summary>
        void OnTestWarning();

        /// <summary>
        /// To test logger.
        /// </summary>
        void OnTestError();
    }
}
