namespace Oxide.Ext.RustApi.Models.Options
{
    public class ApiServerOptions
    {
        private const string DefaultSignHeaderName = "s";

        /// <summary>
        /// Endpoint string. (e.g. http://localhost:6667).
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Secret word to build sign.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Header name to pass sign
        /// </summary>
        public string SignHeaderName { get; set; } = DefaultSignHeaderName;
    }
}
