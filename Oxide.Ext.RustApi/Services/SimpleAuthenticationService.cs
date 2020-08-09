using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    internal class SimpleAuthenticationService : IAuthenticationService
    {
        private const string UserHeaderName = "ra_u";
        private const string SecretHeaderName = "ra_s";

        private readonly RustApiOptions _options;
        private readonly ILogger<SimpleAuthenticationService> _logger;

        public SimpleAuthenticationService(RustApiOptions options, ILogger<SimpleAuthenticationService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public bool TryToGetUser(HttpListenerContext context, out ApiUserInfo userInfo)
        {
            var user = context.Request.Headers[UserHeaderName];
            var secret = context.Request.Headers[SecretHeaderName];

            if (!TryGetUser(user, out userInfo))
            {
                _logger.Warning($"User '{user}' not found.");
                return false;
            }

            // validate args
            if (string.IsNullOrEmpty(secret))
            {
                _logger.Warning($"Current 'secret' value can't be empty for user '{user}'");
                return false;
            }

            // compare signs
            var result = secret.Equals(userInfo.Secret, StringComparison.InvariantCultureIgnoreCase);
            if(!result) _logger.Warning($"Incorrect 'secret' for user '{user}'");

            return result;
        }

        /// <summary>
        /// Looking got user by name.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="userInfo">Found user info.</param>
        /// <returns></returns>
        private bool TryGetUser(string user, out ApiUserInfo userInfo)
        {
            userInfo = default;

            // let's find user options
            userInfo = _options.Users.FirstOrDefault(x => x.Name.Equals(user, StringComparison.InvariantCultureIgnoreCase));
            if (userInfo == default) return false;

            return true;
        }
    }
}
