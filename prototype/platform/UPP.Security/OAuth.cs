using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;

namespace UPP.Security
{
    // Helper class to take care of acquiring tokens from various services
    public sealed class OAuth
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetToken(OAuthProviderConfig config)
        {
            switch (config.Name)
            {
                case "agol":
                    try
                    {
                        // Get a token from ArcGIS Online
                        var client = new RestClient("https://www.arcgis.com");
                        var request = new RestRequest("sharing/rest/oauth2/token/", Method.POST);
                        request.AddParameter("client_id", config.Key);
                        request.AddParameter("client_secret", config.Secret);
                        request.AddParameter("grant_type", "client_credentials");

                        var response = client.Execute(request);
                        logger.Debug("ArcGIS Online response: {0}", response.Content);
                        var payload = JsonConvert.DeserializeObject<AgolOAuthResponse>(response.Content);

                        return payload.access_token;
                    }
                    catch (Exception e)
                    {
                        logger.Warn(e);
                        return null;
                    }

                default:
                    return null;
            }
        }
    }

    public sealed class OAuthProviderConfig
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Scopes { get; set; }
    }

    public sealed class OAuthService
    {
        public OAuthService(string url)
            : this(url, null)
        {
        }

        public OAuthService(string url, string token)
        {
            RouteUrl = url;
            RouteToken = token;
        }

        public string RouteUrl { get; private set; }
        public string RouteToken { get; private set; }
    }

    public sealed class AgolOAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
