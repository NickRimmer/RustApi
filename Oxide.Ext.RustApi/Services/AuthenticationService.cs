using Oxide.Ext.RustApi.Interfaces;
using System;
using System.Linq;
using System.Text;
using Oxide.Ext.RustApi.Models;

namespace Oxide.Ext.RustApi.Services
{
    /// <inheritdoc />
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly RustApiOptions _options;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(RustApiOptions options, ILogger<AuthenticationService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public bool TryToGetUser(string user, string sign, string route, string requestContent, out ApiUserInfo userInfo)
        {
            if (!TryGetUser(user, out userInfo))
            {
                _logger.Warning($"User '{user}' not found.");
                return false;
            }

#if SKIP_SIGN_VALIDATION
            return true;
#endif

            // validate args
            if (string.IsNullOrEmpty(sign))
            {
                _logger.Warning("Current sign value can't be empty");
                return false;
            }

            if (string.IsNullOrEmpty(route))
            {
                _logger.Warning("Route value can't be empty");
                return false;
            }

            // build expected sign
            var expectedSign = BuildSign(route, requestContent, userInfo);

            // compare signs
            return sign.Equals(expectedSign, StringComparison.InvariantCultureIgnoreCase);
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

        /// <summary>
        /// Build sign for request.
        /// </summary>
        /// <param name="route">Route value.</param>
        /// <param name="requestContent">Request content.</param>
        /// <param name="userInfo">Current user info.</param>
        /// <returns></returns>
        private static string BuildSign(string route, string requestContent, ApiUserInfo userInfo)
        {
            // build expected sign
            var str = route + (requestContent?.Trim() ?? string.Empty) + userInfo.Secret;
            var bytes = Encoding.UTF8.GetBytes(str);
            var result = Convert.ToBase64String(bytes);

            return result;
        }
    }
}
