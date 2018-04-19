using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;

namespace UPP.Security
{
    // Helper class to take care of acquiring tokens from normal ArcGIS Server and Portal token endpoints
    public sealed class WebToken
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetToken(TokenProviderConfig config)
        {
            try
            {
                var url = new Uri(config.Url);

                // Get a token from the service
                var baseUrl = String.Format("{0}://{1}", url.Scheme, url.Authority);
                var client = new RestClient(baseUrl);

                var request = new RestRequest(url.PathAndQuery, Method.POST);
                request.AddParameter("f", "json");
                request.AddParameter("username", config.Username);
                request.AddParameter("password", config.Password);
                request.AddParameter("client", "referer");
                request.AddParameter("referer", "https://example.com");

                var response = client.Execute(request);
                logger.Debug("Token response: {0}", response.Content);
                var payload = JsonConvert.DeserializeObject<WebTokenResponse>(response.Content);

                return payload.token;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                return null;
            }
        }
    }
    
    public sealed class WebTokenService
    {
        public WebTokenService(string url)
            : this(url, null)
        {
        }

        public WebTokenService(string url, string token)
        {
            RouteUrl = url;
            RouteToken = token;
        }

        public string RouteUrl { get; private set; }
        public string RouteToken { get; private set; }
    }

    public sealed class WebTokenResponse
    {
        public string token { get; set; }
        public long expires { get; set; }
    }
}
