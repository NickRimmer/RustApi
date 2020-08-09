using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Attributes;
using Oxide.Ext.RustApi.Primitives.Exceptions;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Routes
{
    /// <inheritdoc />
    internal class CommandRoute : RouteBase, ICommandRoute
    {
        private readonly ILogger<CommandRoute> _logger;
        private IReadOnlyList<ApiPlugin> _apiPlugins;

        public CommandRoute(ILogger<CommandRoute> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiPlugins = new ApiPlugin[0];
        }

        /// <inheritdoc />
        public IReadOnlyList<object> OnCallCommand(ApiUserInfo user, ApiCommandRequest request)
        {
            var result = new List<object>();

            // let's inform client in case if there is no any commands
            if (!_apiPlugins.Any()) throw new ApiCommandNotFoundException($"No methods found for the command '{request.CommandName}'");

            foreach (var apiPlugin in _apiPlugins)
            {
                // let's try to find target methods with same command name
                var apiCommands = apiPlugin.Methods

                    // looking for command by name
                    .Where(x => x.ApiInfo.CommandName.Equals(request.CommandName, StringComparison.InvariantCultureIgnoreCase))

                    // filter by required and user permissions
                    .Where(x => IsUserHasAccess(user, x.ApiInfo.RequiredPermissions));

                // in case if command with requested name not found
                if (!apiCommands.Any())
                {
                    var userName = user.IsAnonymous ? "Anonymous" : user.Name;
                    throw new ApiCommandNotFoundException($"Command '{request.CommandName}' not found for user '{userName}'");
                }

                // execute api methods and build response
                foreach (var apiMethod in apiCommands)
                {
                    var methodResult = ExecuteMethod(apiPlugin, apiMethod, request, user);
                    if (methodResult != default) result.Add(methodResult);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public void UpdateApiPluginsCache()
        {
            _apiPlugins = GetApiPlugins();
            var methodsFound = _apiPlugins.Sum(x => x.Methods.Count);
            _logger.Info($"Api methods list updated: {methodsFound}");
        }

        /// <summary>
        /// Execute method.
        /// </summary>
        /// <param name="apiPlugin">Api plugin information.</param>
        /// <param name="apiMethod">Api method information.</param>
        /// <param name="request">Current request.</param>
        /// <param name="user">Current user info.</param>
        /// <returns></returns>
        private object ExecuteMethod(ApiPlugin apiPlugin, ApiMethod apiMethod, ApiCommandRequest request, ApiUserInfo user)
        {
            try
            {
                // build required parameters (in case of unknown type will set as default)
                var parameters = apiMethod
                    .Method
                    .GetParameters()
                    .Select(x =>
                    {
                        if (x.ParameterType == typeof(ApiCommandRequest)) return request;
                        if (x.ParameterType == typeof(ApiUserInfo)) return user;
                        if (x.ParameterType == typeof(ApiCommandAttribute)) return apiMethod.ApiInfo;

                        return default(object);
                    })
                    .ToArray();

                // invoke target method
                var result = apiMethod.Method.Invoke(apiPlugin.Instance, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Api command execution error");
                return default;
            }
        }

        /// <summary>
        /// Looking for plugins with target methods.
        /// </summary>
        /// <returns></returns>
        private static List<ApiPlugin> GetApiPlugins()
        {
            var plugins = Interface.uMod.RootPluginManager.GetPlugins();
            var result = plugins
                .Select(BuildApiPlugin)
                .Where(x => x != default) // filter plugins without target methods
                .ToList();

            return result;
        }

        /// <summary>
        /// Build api plugin information.
        /// </summary>
        /// <param name="plugin">Rust plugin.</param>
        /// <returns></returns>
        private static ApiPlugin BuildApiPlugin(Plugin plugin)
        {
            var methods = plugin
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) // private and public 
                .Select(BuildApiMethod)
                .Where(x => x != default) // filter methods without required attributes
                .ToList();

            if (!methods.Any()) return default;

            var result = new ApiPlugin
            {
                Instance = plugin,
                Methods = methods
            };

            return result;
        }

        /// <summary>
        /// Build api method information.
        /// </summary>
        /// <param name="method">Type method info.</param>
        /// <returns></returns>
        private static ApiMethod BuildApiMethod(MethodInfo method)
        {
            // looking for methods with particular attribute only
            var attribute = method.GetCustomAttribute(typeof(ApiCommandAttribute)) as ApiCommandAttribute;
            if (attribute == null) return default;

            var result = new ApiMethod
            {
                Method = method,
                ApiInfo = attribute
            };

            return result;
        }

        /// <summary>
        /// Api plugin information.
        /// </summary>
        private class ApiPlugin
        {
            public Plugin Instance { get; set; }
            public IReadOnlyList<ApiMethod> Methods { get; set; }
        }

        /// <summary>
        /// Api method information.
        /// </summary>
        private class ApiMethod
        {
            public MethodInfo Method { get; set; }
            public ApiCommandAttribute ApiInfo { get; set; }
        }
    }

    internal static class CommandRouteExtension
    {
        public static MicroContainer AddCommandRoutes(this MicroContainer container)
        {
            container.AddSingle<ISystemRoute, SystemRoute>();
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes.AddRoute<ApiCommandRequest>(
                "command",
                (user, request) => container.Get<ICommandRoute>().OnCallCommand(user, request)
            );

            return container;
        }
    }
}
