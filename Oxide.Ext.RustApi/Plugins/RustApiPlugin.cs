using System;
using Oxide.Core;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Plugins
{
    /// <summary>
    /// General Rust API plugin with hooks
    /// </summary>
    internal class RustApiPlugin : CSharpPlugin
    {
        private readonly RustApiExtension _ext;

        public RustApiPlugin()
        {
            // Set plugin info attributes
            Title = AssemblyInfo.ReadAssemblyAttribute<AssemblyProductAttribute>().Product;
            Author = AssemblyInfo.ReadAssemblyAttribute<AssemblyCompanyAttribute>().Company;
            Version = AssemblyInfo.ReadAssemblyVersion();

            // get access to extension
            _ext = Interface.uMod
                .GetAllExtensions()
                .First(x => x is RustApiExtension) as RustApiExtension;

            // register commands
            RegisterConsoleCommand("api.help", Help);
            RegisterConsoleCommand("api.reload", ReloadCfg);
            RegisterConsoleCommand("api.users", Users);
            RegisterConsoleCommand("api.version", GetVersion);
            RegisterConsoleCommand("api.commands", GetCommands);

            // here can be any commands such user management, log level configuration, etc.
        }

        /// <summary>
        /// Help information
        /// </summary>
        private void Help()
        {
            var messageLines = new List<string>();
            messageLines.AddRange(new[]
            {
                "Console commands:",
                "> api.help - this message",
                $"> api.reload - Reload extenstion configuration from file: {RustApiServices.ConfigFileName}",
                "> api.users - List of registered users",
                "> api.version - Installed version of RustApi extension",
                "> api.commands - List of cached commands",
            });

            Puts(string.Join("\n", messageLines));
        }

        /// <summary>
        /// Reload configuration
        /// </summary>
        private void ReloadCfg() => _ext.ReloadConfiguration();

        /// <summary>
        /// List of registered users
        /// </summary>
        /// <returns></returns>
        private void Users()
        {
            var options = _ext.Container.Get<RustApiOptions>();
            var users = options.Users.Select(x => x.Name);

            Puts(string.Join(", ", users));
        }

        /// <summary>
        /// Help method to register commands
        /// </summary>
        /// <param name="command"></param>
        /// <param name="action"></param>
        private void RegisterConsoleCommand(string command, Action action)
        {
            covalence.RegisterCommand(command, this, (caller, s, args) =>
            {
                if (!caller.IsServer) return false;
                
                action.Invoke();
                return true;
            });
        }

        /// <summary>
        /// Print API extension version
        /// </summary>
        private void GetVersion() => Puts(_ext.Version.ToString());

        /// <summary>
        /// Print list of api commands
        /// </summary>
        private void GetCommands()
        {
            var commands = _ext.Container.Get<ICommandRoute>().CommandsInfo;
            Puts(string.Join(", ", commands));
        }
    }
}
