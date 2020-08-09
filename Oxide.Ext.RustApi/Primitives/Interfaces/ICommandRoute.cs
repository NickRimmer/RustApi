using System.Collections.Generic;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Primitives.Interfaces
{
    /// <summary>
    /// Commands route handler.
    /// </summary>
    public interface ICommandRoute
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
    }
}
