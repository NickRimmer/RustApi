using Newtonsoft.Json;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;
using System;
using System.IO;

namespace Oxide.Ext.RustApi.Business.Common
{
    /// <summary>
    /// Options manager.
    /// </summary>
    internal static class OptionsManager
    {
        /// <summary>
        /// Read from option file.
        /// </summary>
        /// <typeparam name="TOptions">Expected data model.</typeparam>
        /// <param name="optionsFileName">Options file name.</param>
        /// <param name="container">Services container.</param>
        /// <param name="buildDefaultOptions">Default options builder in case if file not exists.</param>
        /// <returns></returns>
        public static TOptions ReadOptions<TOptions>(string optionsFileName, MicroContainer container, Func<MicroContainer, TOptions> buildDefaultOptions = null) where TOptions : class
        {
            var logger = container.GetLogger();
            TOptions options;

            // try to read configuration file
            var path = BuildOptionsPath(optionsFileName, logger);
            if (!File.Exists(path))
            {
                logger.Info($"Options file not found: {optionsFileName}");
                if (buildDefaultOptions == null) return default;

                // generate file with default data
                var defaultOptions = buildDefaultOptions.Invoke(container);
                WriteOptions(optionsFileName, defaultOptions, container);
                return defaultOptions;
            }

            logger.Info($"Read configuration file: {path}");

            // read from file
            var str = File.ReadAllText(path);
            options = JsonConvert.DeserializeObject<TOptions>(str);

            return options;
        }

        /// <summary>
        /// Write to options file.
        /// </summary>
        /// <param name="optionsFileName">Options file name.</param>
        /// <param name="options">Options data.</param>
        /// <param name="container">Services container.</param>
        public static void WriteOptions(string optionsFileName, object options, MicroContainer container)
        {
            var logger = container.GetLogger();
            var path = BuildOptionsPath(optionsFileName, logger);

            var str = JsonConvert.SerializeObject(options, Formatting.Indented);
            File.WriteAllText(path, str);
        }

        /// <summary>
        /// Get logger from DI.
        /// </summary>
        /// <param name="container">Services container.</param>
        /// <returns></returns>
        private static ILogger<RustApiExtension> GetLogger(this MicroContainer container) =>
            container?.Get<ILogger<RustApiExtension>>(false) ?? new UModLogger<RustApiExtension>(new RustApiOptions(string.Empty));

        /// <summary>
        /// Build options path.
        /// </summary>
        /// <param name="optionsFileName">Options file name.</param>
        /// <param name="logger">Logger instance.</param>
        /// <returns></returns>
        private static string BuildOptionsPath(string optionsFileName, ILogger<RustApiExtension> logger)
        {
            var directory = RustApiExtension.OxideHelper.GetInstanceDirectory();
            if (string.IsNullOrEmpty(directory))
            {
                logger.Warning("Oxide instance directory not set, will be used current application directory to read configuration file");
                directory = Directory.GetCurrentDirectory();
            }

            var path = Path.Combine(directory, optionsFileName);
            return path;
        }
    }
}
