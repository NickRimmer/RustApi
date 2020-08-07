using Newtonsoft.Json;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Oxide.Ext.RustApi.Services
{
    /// <summary>
    /// Simple API server.
    /// </summary>
    public class ApiServer : IDisposable
    {
        private readonly ApiServerOptions _options;
        private readonly ILogger<ApiServer> _logger;
        private HttpListener _listener;
        private Dictionary<string, Func<string, object>> _routes;

        public ApiServer(ApiServerOptions options, ILogger<ApiServer> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _listener = new HttpListener();
            _routes = new Dictionary<string, Func<string, object>>();

            SetupListener();
        }

        /// <summary>
        /// Add route with expected response data model.
        /// </summary>
        /// <typeparam name="T">Expected response data model.</typeparam>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        public ApiServer AddRoute<T>(string route, Func<T, object> callback) where T : class
        {
            var url = FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (response) =>
            {
                T data = string.IsNullOrEmpty(response)
                    ? default
                    : JsonConvert.DeserializeObject<T>(response);

                return callback.Invoke(data);
            });

            return this;
        }

        /// <summary>
        /// Add simple route.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        public ApiServer AddRoute(string route, Func<object> callback)
        {
            var url = FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (_) => callback.Invoke());
            return this;
        }

        /// <summary>
        /// Add simple route withour response and request data.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        public ApiServer AddRoute(string route, Action callback)
        {
            var url = FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (_) =>
            {
                callback.Invoke();
                return default;
            });

            return this;
        }

        /// <summary>
        /// Add simple route without response data.
        /// </summary>
        /// <param name="route">Absolute url.</param>
        /// <param name="callback">Callback function.</param>
        /// <returns></returns>
        public ApiServer AddRoute<T>(string route, Action<T> callback)
        {
            var url = FormatUrl(route);
            if (_routes.ContainsKey(url)) throw new ArgumentException($"Route '{route}' already added", nameof(route));

            _routes.Add(url, (response) =>
            {
                T data = string.IsNullOrEmpty(response)
                    ? default
                    : JsonConvert.DeserializeObject<T>(response);

                callback.Invoke(data);
                return default;
            });

            return this;
        }

        /// <summary>
        /// Start server listener.
        /// </summary>
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
        public void Dispose()
        {
            if (_listener?.IsListening == true) _listener.Stop();
            ((IDisposable)_listener)?.Dispose();
            _listener = null;
        }

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

                // validate sign
                var currentSign = context.Request.Headers[_options.SignHeaderName];
                if (!IsValidSign(currentSign, route, requestContent))
                {
                    response.StatusCode = 403;
                    response.Close();
                    return;
                }

                // execute handler
                if (!TryToExecuteHandler(route, requestContent, out var statusCode, out var responseContent))
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
        private bool TryToExecuteHandler(string route, string requestContent, out int statusCode, out object responseContent)
        {
            responseContent = default;
            statusCode = 200;

            if (!_routes.TryGetValue(route, out var routeHandler))
            {
                _logger.Error($"Route not found: {route}");
                statusCode = 400;

                return false;
            }

            try
            {
                responseContent = routeHandler.Invoke(requestContent);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    _logger.Error(ex, $"Route handler ({route}) throw exception cause wrong arguments");
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
                    _logger.Error(ex, $"Route handler ({route}) throw exception");
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

        /// <summary>
        /// Test if sign is valid.
        /// </summary>
        /// <param name="currentSign">Current sign in request.</param>
        /// <param name="route">Route url value.</param>
        /// <param name="requestContent">Content data.</param>
        /// <returns></returns>
        private bool IsValidSign(string currentSign, string route, string requestContent)
        {
#if SKIP_SIGN_VALIDATION
            return true;
#endif

            // validate args
            if (string.IsNullOrEmpty(currentSign))
            {
                _logger.Warning("Current sign value can't be empty");
                return false;
            }

            if (string.IsNullOrEmpty(route))
            {
                _logger.Warning("Route value can't be empty");
                return false;
            }

            if (string.IsNullOrEmpty(_options.Secret))
            {
                _logger.Warning("Secret options can't be empty");
                return false;
            }

            // build expected sign
            var str = route + (requestContent?.Trim() ?? string.Empty) + _options.Secret;
            var bytes = Encoding.UTF8.GetBytes(str);
            var expectedSign = Convert.ToBase64String(bytes);

            // compare signs
            return currentSign.Equals(expectedSign, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// To generalize route
        /// </summary>
        /// <param name="url">URL string</param>
        /// <returns></returns>
        private static string FormatUrl(string url) => url.Trim('/', ' ', '?');
    }
}
