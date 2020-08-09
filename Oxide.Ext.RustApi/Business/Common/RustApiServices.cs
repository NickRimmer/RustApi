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
        internal const string ConfigFileName = "rust-api.config.json";
        private const string DefaultEndpoint = "http://*:28017";

        /// <summary>
        /// Build micro container with required services
        /// </summary>
        /// <returns></returns>
        public static MicroContainer AddRustApiServices(this MicroContainer container)
        {
            container
                .Add(typeof(ILogger<>), typeof(UModLogger<>))
                .AddSingle(GetOptions()) //TODO read from configuration
                .AddSingle<IApiServer, ApiServer>()
                .AddSingle<IAuthenticationService, SimpleAuthenticationService>()
                .AddSingle<IApiRoutes, ApiRoutes>()
                .AddSingle<RustApiPlugin>();

            container
                .AddHookRoutes()
                .AddCommandRoutes()
                .AddSystemRoutes();

            return container;
        }

        private static RustApiOptions GetOptions()
        {
            RustApiOptions options;

            // try to read configuration file
            var path = Path.Combine(Interface.uMod.InstanceDirectory, ConfigFileName);
            if (File.Exists(path))
            {
                // read from file
                var str = File.ReadAllText(path);
                options = JsonConvert.DeserializeObject<RustApiOptions>(str);
            }
            else
            {
                // set and store default
                var firstUser = new ApiUserInfo("admin", "secret", new List<string> { "admin" });
                options = new RustApiOptions(DefaultEndpoint, new List<ApiUserInfo> { firstUser });

                var str = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(path, str);
            }

            return options;
        }
    }
}
