using Oxide.Core.Extensions;
using Oxide.Core.Logging;
using System;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Tests.FakeServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to stop.\n");
            Console.ResetColor();

            RustApiExtension.OxideHelper = new FakeOxide();
            var extensionManager = BuildExtensionsManager();
            var extension = new RustApiExtension(extensionManager);

            // to load plugins
            extension.Load();

            // to start server
            extension.OnModLoad();

            Console.ReadKey();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nShutting down...\n");
            Console.ResetColor();

            extension.OnShutdown();

            //Environment.Exit(-1);
        }

        private static ExtensionManager BuildExtensionsManager()
        {
            var logger = new CompoundLogger();
            var result = new ExtensionManager(logger);
            return result;
        }
    }
}
