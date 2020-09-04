using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using System;
using System.Linq;
using System.Net;

namespace Oxide.Ext.RustApi.Business.Routes
{
    /// <inheritdoc cref="IAuthRoute"/>
    internal class AuthRoute : RouteBase, IAuthRoute
    {
        public const string LoginRoute = "auth/login";
        public const string ConfirmRoute = "auth/confirm";
        public const string CallbackRoute = "auth/steamId";

        private const string CustomCallbackQueryParam = "callback";

        private readonly ISteamConnection _steamConnection;
        private readonly ILogger<AuthRoute> _logger;
        private readonly IAuthenticationService _authService;

        public AuthRoute(ISteamConnection steamConnection, ILogger<AuthRoute> logger, IAuthenticationService authService)
        {
            _steamConnection = steamConnection ?? throw new ArgumentNullException(nameof(steamConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <inheritdoc />
        public Uri Login(HttpListenerContext context)
        {
            var callbackUri = BuildValidationUrl(context);
            var steamUrl = _steamConnection.GetLoginUrl(context, callbackUri);

            var result = new Uri(steamUrl);
            return result;
        }

        /// <inheritdoc />
        public Uri Confirm(HttpListenerContext context)
        {
            var query = context.Request.Url.Query.TrimStart('?');
            var data = query
                .Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(x => x.First(), x => x.Last());

            if (!data.ContainsKey("openid.identity"))
            {
                _logger.Warning("Incorrect login response on validation");
                return default;
            }

            var steamId = _steamConnection.GetSteamId(data);
            var isValid = !string.IsNullOrEmpty(steamId);

            // check if response is valid
            if (!isValid)
            {
                _logger.Warning($"Invalid login (steam response)");
                return BuildCallbackUrl(context, null, null);
            }
            else _logger.Debug($"Logged user with steam ID: {steamId}");

            // generate secrets
            var playerSecret = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var playerInfo = _authService.AddUser(steamId, playerSecret, AuthenticationService.PlayerPermission);
            _logger.Debug($"Added new user with player permission: {playerInfo.Secret}");

            var result = BuildCallbackUrl(context, playerInfo.Name, playerInfo.Secret);
            return result;
        }

        /// <summary>
        /// Build url for validation.
        /// </summary>
        /// <param name="context">Request context.</param>
        /// <returns></returns>
        private static Uri BuildValidationUrl(HttpListenerContext context)
        {
            // try to read custom callback value from GET params
            var customCallback = context.Request.QueryString.Get(CustomCallbackQueryParam);

            // setup callback url
            var result = BuildUri(context, ConfirmRoute, $"{CustomCallbackQueryParam}={customCallback}");

            return result;
        }

        /// <summary>
        /// Build url for callback.
        /// </summary>
        /// <param name="context">Request context.</param>
        /// <param name="name">Player name value.</param>
        /// <param name="secret">Player secret value. If empty will be generated url with error message.</param>
        /// <returns></returns>
        private static Uri BuildCallbackUrl(HttpListenerContext context, string name, string secret)
        {
            // try to read custom callback value from GET params
            var customCallback = context.Request.QueryString.Get(CustomCallbackQueryParam);
            var query = string.IsNullOrEmpty(secret)
                ? "error=invalid_login" // if steam wasn't confirm authorization
                : $"name={name}&secret={secret}"; // otherwise we will send super secret word

            // setup callback url
            var result = string.IsNullOrEmpty(customCallback)
                ? BuildUri(context, CallbackRoute, query) // to internal callback 
                : new UriBuilder(customCallback) { Query = query }.Uri; // to external callback

            return result;
        }

        /// <summary>
        /// Build url to RustApi endpoint.
        /// </summary>
        /// <param name="context">Request context.</param>
        /// <param name="path">Url path.</param>
        /// <param name="query">Query arguments.</param>
        /// <returns></returns>
        private static Uri BuildUri(HttpListenerContext context, string path, string query = null) => new UriBuilder
        {
            Scheme = context.Request.Url.Scheme,
            Host = context.Request.Url.Host,
            Port = context.Request.Url.Port,
            Path = path,
            Query = query
        }.Uri;
    }

    internal static class AuthRouteExtension
    {
        public static MicroContainer AddAuthRoutes(this MicroContainer container)
        {
            container.AddSingle<IAuthRoute, AuthRoute>();
            var apiRoutes = container.Get<IApiRoutes>();

            apiRoutes.AddRoute(AuthRoute.LoginRoute, args => container.Get<IAuthRoute>().Login(args.Context), true);
            apiRoutes.AddRoute(AuthRoute.ConfirmRoute, args => container.Get<IAuthRoute>().Confirm(args.Context), true);
            apiRoutes.AddRoute(AuthRoute.CallbackRoute, args => null, true); // empty route just for callback from steam

            return container;
        }
    }
}
