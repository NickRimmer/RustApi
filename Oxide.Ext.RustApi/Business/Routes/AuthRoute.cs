using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using System;
using System.Linq;
using System.Net;
using Oxide.Ext.RustApi.Business.Services;

namespace Oxide.Ext.RustApi.Business.Routes
{
    internal class AuthRoute : RouteBase, IAuthRoute
    {
        public static string LoginRoute = $"auth/login";
        public static string ConfirmRoute = $"auth/confirm";
        public static string SteamIdRoute = "auth/steamId";

        private readonly ISteamConnection _steamConnection;
        private readonly ILogger<AuthRoute> _logger;
        private readonly IAuthenticationService _authService;

        public AuthRoute(ISteamConnection steamConnection, ILogger<AuthRoute> logger, IAuthenticationService authService)
        {
            _steamConnection = steamConnection ?? throw new ArgumentNullException(nameof(steamConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public Uri Login(HttpListenerContext context)
        {
            var callbackUri = BuildUri(context, ConfirmRoute);
            var steamUrl = _steamConnection.GetLoginUrl(context, callbackUri);

            var result = new Uri(steamUrl);
            return result;
        }

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

            if (!isValid) _logger.Warning($"Invalid login (steam response)");
            else _logger.Debug($"Logged user with steam ID: {steamId}");

            var playerSecret = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var playerInfo = _authService.AddUser(steamId, playerSecret, SimpleAuthenticationService.PlayerPermission);
            _logger.Debug($"Added new user with player permission: {playerInfo.Secret}");

            var result = BuildUri(context, $"{SteamIdRoute}", $"key={playerInfo.Secret}");
            return result;
        }

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
            apiRoutes.AddRoute(AuthRoute.SteamIdRoute, args => null);

            return container;
        }
    }
}
