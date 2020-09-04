using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class AuthenticationService : IAuthenticationService
    {
        public const string UsersFileName = "rust-api.users.json";
        public const string AdminPermission = "admin";
        public const string PlayerPermission = "player";

        private const string UserHeaderName = "ra_u";
        private const string SecretHeaderName = "ra_s";

        private readonly ILogger<AuthenticationService> _logger;
        private readonly MicroContainer _container;

        private List<ApiUserInfo> _users;

        public AuthenticationService(ILogger<AuthenticationService> logger, MicroContainer container)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            
            ReloadUsers();
        }

        /// <inheritdoc />
        public void ReloadUsers()
        {
            _users = OptionsManager.ReadOptions<List<ApiUserInfo>>(UsersFileName, _container, BuildDefaultUsersList);
        }

        /// <inheritdoc />
        public bool TryToGetUser(HttpListenerContext context, out ApiUserInfo userInfo)
        {
            var user = context.Request.Headers[UserHeaderName];
            var secret = context.Request.Headers[SecretHeaderName];

            // if user name wasn't sent
            if (string.IsNullOrEmpty(user))
            {
                userInfo = ApiUserInfo.Anonymous;
                return true;
            }

            // try to find user info
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
            if (!result) _logger.Warning($"Incorrect 'secret' for user '{user}'");

            return result;
        }

        /// <inheritdoc />
        public bool TryToGetUser(string name, out ApiUserInfo userInfo)
        {
            if (string.IsNullOrEmpty(name))
            {
                userInfo = ApiUserInfo.Anonymous;
                return false;
            }

            // try to find user info
            if (!TryGetUser(name, out userInfo))
            {
                _logger.Warning($"User '{name}' not found.");
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public ApiUserInfo AddUser(string name, string secret, params string[] permissions)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            var exist = _users.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            // remove exist player info
            if (exist != default) _users.Remove(exist);

            var userPermissions = exist?.Permissions ?? new List<string>();

            // setup permissions
            foreach (var permission in permissions)
                if (!userPermissions.Contains(permission, StringComparer.InvariantCultureIgnoreCase)) userPermissions.Add(permission);

            // add user
            var result = new ApiUserInfo(name, secret, userPermissions);
            _users.Add(result);
            OptionsManager.WriteOptions(UsersFileName, _users, _container);

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
            userInfo = _users.FirstOrDefault(x => x.Name.Equals(user, StringComparison.InvariantCultureIgnoreCase));
            if (userInfo == default) return false;

            return true;
        }

        /// <summary>
        /// Build default list of users.
        /// </summary>
        /// <param name="container">Services container.</param>
        /// <returns></returns>
        private List<ApiUserInfo> BuildDefaultUsersList(MicroContainer container)
        {
            var result = new List<ApiUserInfo>
            {
                new ApiUserInfo("admin", Guid.NewGuid().ToString(), new List<string> {AdminPermission}),
            };

            return result;
        }
    }
}
