using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Exceptions;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.Core.Tracing;
using UPP.SimpleAuthentication.Providers.ArcGISOnline;

namespace UPP.SimpleAuthentication.Providers
{
    public class ArcGISOnlineProvider : BaseOAuth20Provider<AccessTokenResult>
    {
        private const string AccessTokenKey = "token";

        public ArcGISOnlineProvider(ProviderParams providerParams) : this("ArcGISOnline", providerParams)
        {
        }

        protected ArcGISOnlineProvider(string name, ProviderParams providerParams) : base(name, providerParams)
        {
            AuthenticateRedirectionUrl = new Uri("https://www.arcgis.com/sharing/oauth2/authorize");
        }

        #region BaseOAuth20Token<AccessTokenResult> Implementation

        public override IEnumerable<string> DefaultScopes
        {
            get { return new[] { "user:email" }; }
        }

        protected override IRestResponse<AccessTokenResult> ExecuteRetrieveAccessToken(string authorizationCode,
                                                                                       Uri redirectUri)
        {
            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentNullException("authorizationCode");
            }

            if (redirectUri == null ||
                string.IsNullOrEmpty(redirectUri.AbsoluteUri))
            {
                throw new ArgumentNullException("redirectUri");
            }

            var restRequest = new RestRequest("/sharing/rest/oauth2/token", Method.POST);
            restRequest.AddParameter("client_id", PublicApiKey);
            restRequest.AddParameter("code", authorizationCode);
            restRequest.AddParameter("redirect_uri", redirectUri.AbsoluteUri);
            restRequest.AddParameter("grant_type", "authorization_code");

            var restClient = RestClientFactory.CreateRestClient("https://www.arcgis.com/");
            TraceSource.TraceVerbose("Retrieving Access Token endpoint: {0}",
                                     restClient.BuildUri(restRequest).AbsoluteUri);

            // ArcGIS Online returns the token as text/plain, but it's actually a JSON payload
            restClient.AddHandler("text/plain", new RestSharp.Deserializers.JsonDeserializer());

            var response = restClient.Execute<AccessTokenResult>(restRequest);
            return response;
        }

        protected override AccessToken MapAccessTokenResultToAccessToken(AccessTokenResult accessTokenResult)
        {
            if (accessTokenResult == null)
            {
                throw new ArgumentNullException("accessTokenResult");
            }

            if (string.IsNullOrEmpty(accessTokenResult.AccessToken))
            {
                var errorMessage =
                    string.Format(
                        "Retrieved an ArcGIS Online Access Token but it doesn't contain: {0}",
                        AccessTokenKey);
                TraceSource.TraceError(errorMessage);
                throw new AuthenticationException(errorMessage);
            }

            return new AccessToken
            {
                PublicToken = accessTokenResult.AccessToken,
                ExpiresOn = DateTime.UtcNow.AddSeconds(accessTokenResult.ExpiresIn)
            };
        }

        protected override UserInformation RetrieveUserInformation(AccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException("accessToken");
            }

            if (string.IsNullOrEmpty(accessToken.PublicToken))
            {
                throw new ArgumentException("accessToken.PublicToken");
            }

            IRestResponse<UserInfoResult> response;

            try
            {
                var restRequest = new RestRequest("/sharing/rest/community/self", Method.GET);
                restRequest.AddParameter(AccessTokenKey, accessToken.PublicToken);
                restRequest.AddParameter("f", "json");

                // ArcGIS Online returns the token as text/plain, but it's actually a JSON payload
                var restClient = RestClientFactory.CreateRestClient("https://www.arcgis.com");
                restClient.AddHandler("text/plain", new RestSharp.Deserializers.JsonDeserializer());
                restClient.UserAgent = PublicApiKey;

                TraceSource.TraceVerbose("Retrieving user information. ArcGIS Online Endpoint: {0}",
                                         restClient.BuildUri(restRequest).AbsoluteUri);

                response = restClient.Execute<UserInfoResult>(restRequest);
            }
            catch (Exception exception)
            {
                throw new AuthenticationException("Failed to obtain User Info from ArcGIS Online.", exception);
            }

            if (response == null ||
                response.StatusCode != HttpStatusCode.OK)
            {
                throw new AuthenticationException(
                    string.Format(
                        "Failed to obtain User Info from ArcGIS Online OR the the response was not an HTTP Status 200 OK. Response Status: {0}. Response Description: {1}",
                        response == null ? "-- null response --" : response.StatusCode.ToString(),
                        response == null ? string.Empty : response.StatusDescription));
            }

            return new UserInformation
            {
                Id = response.Data.UserName,
                Name = response.Data.FullName,
                Email = response.Data.Email,
                UserName = response.Data.UserName
            };
        }

        #endregion
    }
}
