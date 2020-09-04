using Oxide.Ext.RustApi.Primitives.Models;
using System.Collections.Generic;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Commands route handler.
    /// </summary>
    internal interface ICommandRoute
    {
        /// <summary>
        /// On command api call.
        /// </summary>
        /// <param name="user">Current user.</param>
        /// <param name="request">Current request.</param>
        /// <returns></returns>
        IReadOnlyList<object> OnCallCommand(ApiUserInfo user, ApiCommandRequest request);

        /// <summary>
        /// Update list of Api plugins
        /// </summary>
        void UpdateApiPluginsCache();

        /// <summary>
        /// Available commands list.
        /// </summary>
        IReadOnlyList<string> CommandsInfo { get; }
    }
}
