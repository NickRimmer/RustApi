using System;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Ext.RustApi.Business.Routes;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Plugins;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;
using System.Collections.Generic;
using System.IO;

namespace Oxide.Ext.RustApi.Business.Common
{
    internal static class RustApiServices
    {
        internal const string DefaultConfigFileName = "rust-api.config.json";
        private const string DefaultEndpoint = "http://*:28017";

        /// <summary>
        /// Build micro container with required services
        /// </summary>
        /// <returns></returns>
        public static MicroContainer AddRustApiServices(this MicroContainer container)
        {
            container.AddOptions();

            container
                .Add(typeof(ILogger<>), typeof(UModLogger<>))
                .AddSingle<IApiServer, ApiServer>()
                .AddSingle<IAuthenticationService, SimpleAuthenticationService>()
                .AddSingle<RustApiPlugin>();

            container
                .AddSingle<IApiRoutes, ApiRoutes>()
                .AddHookRoutes()
                .AddCommandRoutes()
                .AddSystemRoutes();

            return container;
        }

        /// <summary>
        /// Add extension options
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static MicroContainer AddOptions(this MicroContainer container) => container.AddSingle(GetOptions(container));

        /// <summary>
        /// Read options method
        /// </summary>
        /// <param name="configFileName">Configuration file name</param>
        /// <returns></returns>
        private static RustApiOptions GetOptions(MicroContainer container, string configFileName = DefaultConfigFileName)
        {
            var logger = container.Get<ILogger<RustApiExtension>>(false) ?? new UModLogger<RustApiExtension>(new RustApiOptions(string.Empty));
            RustApiOptions options;
            
            var directory = RustApiExtension.OxideHelper.GetInstanceDirectory();
            if (string.IsNullOrEmpty(directory))
            {
                logger.Warning("Oxide instance directory not set, will be used current application directory to read configuration file");
                directory = Directory.GetCurrentDirectory();
            }

            // try to read configuration file
            var path = Path.Combine(directory, configFileName);
            if (File.Exists(path))
            {
                logger.Info($"Read configuration file: {path}");

                // read from file
                var str = File.ReadAllText(path);
                options = JsonConvert.DeserializeObject<RustApiOptions>(str);
            }
            else
            {
                logger.Info($"Create configuration file: {path}");

                // set and store default
                options = new RustApiOptions(DefaultEndpoint, new List<ApiUserInfo>
                {
                    new ApiUserInfo("admin", Guid.NewGuid().ToString(), new List<string> { "admin" }),
                });

                var str = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(path, str);
            }

            return options;
        }
    }
}
