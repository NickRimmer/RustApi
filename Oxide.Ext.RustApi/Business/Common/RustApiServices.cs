﻿using Oxide.Ext.RustApi.Business.Routes;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Plugins;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Common
{
    internal static class RustApiServices
    {
        internal const string ConfigFileName = "rust-api.config.json";
        private const string DefaultEndpoint = "http://*:28017";

        /// <summary>
        /// Build micro container with required services
        /// </summary>
        /// <returns></returns>
        public static MicroContainer AddRustApiServices(this MicroContainer container)
        {
            // add API options
            container.LoadApiOptions();

            // add services
            container
                .Add(typeof(ILogger<>), typeof(UModLogger<>))
                .AddSingle<IApiServer, ApiServer>()
                .AddSingle<ISteamConnection, SteamConnection>()
                .AddSingle<IAuthenticationService, AuthenticationService>()
                .AddSingle<RustApiPlugin>();

            // add routes
            container
                .AddSingle<IApiRoutes, ApiRoutes>()
                .AddHookRoutes()
                .AddCommandRoutes()
                .AddSystemRoutes()
                .AddAuthRoutes();

            return container;
        }

        public static MicroContainer LoadApiOptions(this MicroContainer container) => container.AddSingle(GetApiOptions(container));

        /// <summary>
        /// Read options method.
        /// </summary>
        /// <param name="container">Services container.</param>
        /// <param name="configFileName">Configuration file name.</param>
        /// <returns></returns>
        private static RustApiOptions GetApiOptions(MicroContainer container)
        {
            var options = OptionsManager.ReadOptions<RustApiOptions>(ConfigFileName, container);
            if (options == default)
            {
                options = new RustApiOptions(DefaultEndpoint);
                OptionsManager.WriteOptions(ConfigFileName, options, container);
            }

            return options;
        }
    }
}
