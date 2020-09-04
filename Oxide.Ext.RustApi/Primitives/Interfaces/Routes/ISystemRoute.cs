using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// System and test routes.
    /// </summary>
    internal interface ISystemRoute
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

        /// <summary>
        /// Get user info.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        object OnUserInfo(ApiUserInfo user);
    }
}
