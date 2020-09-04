using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Oxide.Ext.RustApi.Business.Routes;
using Oxide.Ext.RustApi.Primitives.Interfaces;

namespace Oxide.Ext.RustApi.Business.Services
{
    /// <inheritdoc />
    internal class SteamConnection: ISteamConnection
    {
        /// <summary>
        /// Steam base url
        /// </summary>
        private const string SteamBaseUrl = "https://steamcommunity.com/openid/login";

        /// <inheritdoc />
        public string GetLoginUrl(HttpListenerContext context, Uri callbackUri)
        {
            // build steam login url
            var queryParams = new Dictionary<string, string>
            {
                {"openid.ns", "http://specs.openid.net/auth/2.0"},
                {"openid.mode", "checkid_setup"},
                {"openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"},
                {"openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"},

                {"openid.return_to", callbackUri.ToString()},
                {"openid.realm", $"{callbackUri.Scheme}://{callbackUri.Authority}"}
            };

            var query = BuildQuery(queryParams);
            var result = $"{SteamBaseUrl}?{query}";

            return result;
        }

        /// <inheritdoc />
        public string GetSteamId(Dictionary<string, string> steamResponse)
        {
            if (!steamResponse.ContainsKey("openid.identity")) return string.Empty;

            var queryParams = steamResponse;
            queryParams["openid.mode"] = "check_authentication";

            var data = new NameValueCollection();
            foreach (var p in queryParams)
                data.Add(p.Key, HttpUtility.UrlDecode(p.Value));

            using (var wc = new WebClient())
            {
                var result = wc.UploadValues(SteamBaseUrl, data);
                var resultStr = Encoding.UTF8.GetString(result);

                if (!resultStr.Contains("is_valid:true")) return string.Empty;
                var steamId = HttpUtility.UrlDecode(steamResponse["openid.identity"]).Split('/').Last();

                return steamId;
            }
        }

        private static string BuildQuery(Dictionary<string, string> queryParams)
        {
            var result = string.Join("&", queryParams
                .OrderBy(x => x.Key)
                .Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}"));

            return result;
        }
    }
}
