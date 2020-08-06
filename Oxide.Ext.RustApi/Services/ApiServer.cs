using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models.Options;

namespace Oxide.Ext.RustApi.Services
{
    /// <summary>
    /// Simple API server.
    /// </summary>
    public class ApiServer : IDisposable
    {
        private readonly ILogger<ApiServer> _logger;
        private HttpListener _listener;
        private Dictionary<string, Func<string, object>> _routes;

        public ApiServer(ApiServerOptions options, ILogger<ApiServer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _listener = new HttpListener();
            _routes = new Dictionary<string, Func<string, object>>();

            SetupListener(options);
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

            _logger.Info($"Api route: {url}");
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

            _logger.Info($"Api route: {url}");
            return this;
        }

        /// <summary>
        /// Add simple route.
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

            _logger.Info($"Api route: {url}");
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
        /// <param name="options"></param>
        private void SetupListener(ApiServerOptions options)
        {
            var prefix = options.Endpoint;
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
                // try to find route handler
                var route = FormatUrl(context.Request.Url.AbsolutePath);
                if (!_routes.TryGetValue(route, out var routeHandler))
                    throw new KeyNotFoundException($"Route not found: {route}");

                // handle request
                var response = context.Response;

                // try to read request body
                if (!TryToReadBody(context.Request, out var requestContent))
                {
                    response.StatusCode = 500;
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
        /// To generalize route
        /// </summary>
        /// <param name="url">URL string</param>
        /// <returns></returns>
        private static string FormatUrl(string url) => url.Trim('/', ' ', '?');
    }
}
