﻿namespace Oxide.Ext.RustApi.Interfaces
{
    /// <summary>
    /// Simple API server.
    /// </summary>
    internal interface IApiServer
    {
        /// <summary>
        /// Start server listener.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop server listener.
        /// </summary>
        void Destroy();
    }
}
