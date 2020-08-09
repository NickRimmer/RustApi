using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Oxide.Ext.RustApi.Primitives.Exceptions;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class ApiServer : IApiServer
    {
        private readonly RustApiOptions _options;
        private readonly ILogger<ApiServer> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IApiRoutes _apiRoutes;
        private HttpListener _listener;

        public ApiServer(
            RustApiOptions options,
            ILogger<ApiServer> logger,
            IAuthenticationService authenticationService,
            IApiRoutes apiRoutes)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _apiRoutes = apiRoutes ?? throw new ArgumentNullException(nameof(apiRoutes));
            _listener = new HttpListener();

            SetupListener();
        }

        /// <inheritdoc />
        public void Start()
        {
            if (_listener == null)
            {
                _logger.Error("Can't start listener cause disposed");
                return;
            }

            try
            {
                if (!_listener.IsListening) _listener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't start listener cause: {ex.Message}");
                return;
            }

            // let's wait someone
            ThreadPool.QueueUserWorkItem((_) =>
            {
                while (_listener != null)
                {
                    var context = _listener.GetContext();
                    HandleRequest(context);
                }
            });
        }

        /// <inheritdoc />
        public void Destroy()
        {
            if (_listener?.IsListening == true) _listener.Stop();
            ((IDisposable)_listener)?.Dispose();
            _listener = null;
        }

        /// <summary>
        /// To generalize route
        /// </summary>
        /// <param name="url">URL string</param>
        /// <returns></returns>
        public static string FormatUrl(string url) => url.Trim('/', ' ', '?');

        /// <summary>
        /// Configure listener.
        /// </summary>
        private void SetupListener()
        {
            var prefix = _options.Endpoint;
            if (!prefix.EndsWith("/")) prefix += "/";

            _listener.Prefixes.Add(prefix);

            _logger.Info($"Api server: {prefix}");
        }

        /// <summary>
        /// On API request handler.
        /// </summary>
        /// <param name="context"></param>
        private void HandleRequest(HttpListenerContext context)
        {
            ThreadPool.QueueUserWorkItem((_) =>
            {
                var response = context.Response;

                // only post methods allowed
                if (!context.Request.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
                {
                    response.StatusCode = 405; // method not allowed
                    response.Close();
                    return;
                }

                // try to find route handler
                var route = FormatUrl(context.Request.Url.AbsolutePath);

                // try to read request body
                if (!TryToReadBody(context.Request, out var requestContent))
                {
                    response.StatusCode = 500;
                    response.Close();
                    return;
                }

                // validate user
                if (!_authenticationService.TryToGetUser(context, out var userInfo))
                {
                    response.StatusCode = 403;
                    response.Close();
                    return;
                }

                // execute handler
                if (!TryToExecuteHandler(userInfo, route, requestContent, out var statusCode, out var responseContent))
                {
                    response.StatusCode = statusCode;
                    response.Close();
                    return;
                }

                // if handler result not empty, build response body
                if (responseContent != default)
                {
                    if (!TryToWriteResponse(responseContent, context.Response))
                    {
                        response.StatusCode = 500;
                        response.Close();
                        return;
                    }

                    response.StatusCode = 200; // OK
                    response.Close();
                }

                // otherwise send "No Content" status response
                else
                {
                    response.StatusCode = 204; // No Content
                    response.Close();
                }
            });
        }

        /// <summary>
        /// Try to read request body.
        /// </summary>
        /// <param name="request">Listener request.</param>
        /// <param name="bodyContent">Content body result.</param>
        /// <returns></returns>
        private bool TryToReadBody(HttpListenerRequest request, out string bodyContent)
        {
            bodyContent = string.Empty;

            try
            {
                using (var receiveStream = request.InputStream)
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    bodyContent = readStream.ReadToEnd();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't read request body");
                return false;
            }
        }

        /// <summary>
        /// Try to execute route handler.
        /// </summary>
        /// <param name="route">Route name.</param>
        /// <param name="requestContent">Request content.</param>
        /// <param name="statusCode">Result http status code.</param>
        /// <param name="responseContent">Response body.</param>
        /// <returns></returns>
        private bool TryToExecuteHandler(ApiUserInfo userInfo, string route, string requestContent, out int statusCode, out object responseContent)
        {
            responseContent = default;
            statusCode = 200;

            if (!_apiRoutes.TryGetHandler(route, out var routeHandler))
            {
                _logger.Error($"Route not found: {route}");
                statusCode = 404;

                return false;
            }

            try
            {
                responseContent = routeHandler.Invoke(userInfo, requestContent);
            }
            catch (Exception ex)
            {
                if (ex is ApiCommandNotFoundException)
                {
                    _logger.Warning(ex.Message);
                    statusCode = 404;
                }
                else if (ex is SecurityException)
                {
                    _logger.Warning($"User '{userInfo.Name}' access error");
                    statusCode = 403;
                }
                else if (ex is ArgumentException || ex is TargetInvocationException)
                {
                    _logger.Warning($"Route handler ({route}) throw exception cause wrong arguments");
                    statusCode = 400;
                }
                else if (ex is JsonReaderException || ex is JsonSerializationException)
                {
                    _logger.Error($"Can't deserialize request body to expected type");
                    _logger.Error(ex.Message);
                    statusCode = 400;
                }
                else
                {
                    _logger.Error(ex, $"Route handler ({route}) throw exception ({ex.GetType().Name})");
                    statusCode = 500;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to send response.
        /// </summary>
        /// <param name="responseContent">Response content.</param>
        /// <param name="response">Listener response.</param>
        /// <returns></returns>
        private bool TryToWriteResponse(object responseContent, HttpListenerResponse response)
        {
            try
            {
                var responseContentString = JsonConvert.SerializeObject(responseContent);
                var responseContentBytes = Encoding.UTF8.GetBytes(responseContentString);

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "application/json";
                response.ContentLength64 = responseContentBytes.Length;

                var output = response.OutputStream;
                output.Write(responseContentBytes, 0, responseContentBytes.Length);
                output.Close();
            }
            catch (Exception ex)
            {
                _logger.Error("Can't write response");
                _logger.Error(ex);

                return false;
            }

            return true;
        }
    }
}
