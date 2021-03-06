﻿using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Exceptions;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.Core.Tracing;
using UPP.SimpleAuthentication.Providers.RTVision;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UPP.SimpleAuthentication.Providers
{
    public class RTVisionProvider : BaseOAuth20Provider<AccessTokenResult>
    {
        private const string AccessTokenKey = "access_token";
        private const string TokenTypeKey = "token_type";

        public RTVisionProvider(ProviderParams providerParams) : this("RTVision", providerParams)
        {
        }

        protected RTVisionProvider(string name, ProviderParams providerParams) : base(name, providerParams)
        {
            AuthenticateRedirectionUrl = new Uri("https://staging-upp.onegov.rtvision.com/core/oauth/auth_code.php");
        }

        #region BaseOAuth20Token<AccessTokenResult> Implementation

        public override IEnumerable<string> DefaultScopes
        {
            get { return new[] { "basic","email","company","insurance","vehicle","truck","apply" }; }
        }

        public override string ScopeSeparator
        {
            get { return " "; }
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

            var restRequest = new RestRequest("/core/oauth/auth_code.php", Method.POST);
            restRequest.AddParameter("client_id", PublicApiKey);
            restRequest.AddParameter("client_secret", SecretApiKey);
            restRequest.AddParameter("redirect_uri", redirectUri.AbsoluteUri);
            restRequest.AddParameter("code", authorizationCode);
            restRequest.AddParameter("grant_type", "authorization_code");

            var restClient = RestClientFactory.CreateRestClient("https://staging-upp.onegov.rtvision.com");
            TraceSource.TraceVerbose("Retrieving Access Token endpoint: {0}",
                                     restClient.BuildUri(restRequest).AbsoluteUri);

            return restClient.Execute<AccessTokenResult>(restRequest);
        }

        protected override AccessToken MapAccessTokenResultToAccessToken(AccessTokenResult accessTokenResult)
        {
            if (accessTokenResult == null)
            {
                throw new ArgumentNullException("accessTokenResult");
            }

            if (string.IsNullOrEmpty(accessTokenResult.AccessToken) ||
                string.IsNullOrEmpty(accessTokenResult.TokenType))
            {
                var errorMessage =
                    string.Format(
                        "Retrieved an RTVision Access Token but it doesn't contain one or more of either: {0} or {1}.",
                        AccessTokenKey, TokenTypeKey);
                TraceSource.TraceError(errorMessage);
                throw new AuthenticationException(errorMessage);
            }

            return new AccessToken
            {
                PublicToken = accessTokenResult.AccessToken
                //ExpiresOn = DateTime.UtcNow.AddSeconds(accessTokenResult.ExpiresIn)
            };
        }

        protected override UserInformation RetrieveUserInformation(AccessToken accessToken)
        {
            return new UserInformation
            {
                Id = "rtvision",
                Name = "R. T. Vision",
                Email = "rt@vision.com",
                UserName = "rtUser"
            };
        }

        /*
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
                var restRequest = new RestRequest("/core/oauth/user", Method.GET);
                restRequest.AddParameter(AccessTokenKey, accessToken.PublicToken);

                var restClient = RestClientFactory.CreateRestClient("https://staging-upp.onegov.rtvision.com");

                restClient.UserAgent = PublicApiKey;

                TraceSource.TraceVerbose("Retrieving user information. RTVision Endpoint: {0}",
                                         restClient.BuildUri(restRequest).AbsoluteUri);

                response = restClient.Execute<UserInfoResult>(restRequest);
            }
            catch (Exception exception)
            {
                throw new AuthenticationException("Failed to obtain User Info from RTVision.", exception);
            }

            if (response == null ||
                response.StatusCode != HttpStatusCode.OK)
            {
                throw new AuthenticationException(
                    string.Format(
                        "Failed to obtain User Info from RTVision OR the the response was not an HTTP Status 200 OK. Response Status: {0}. Response Description: {1}",
                        response == null ? "-- null response --" : response.StatusCode.ToString(),
                        response == null ? string.Empty : response.StatusDescription));
            }

            // Lets check to make sure we have some bare minimum data.
            if (string.IsNullOrEmpty(response.Data.Id.ToString()) ||
                string.IsNullOrEmpty(response.Data.Login))
            {
                throw new AuthenticationException(
                    string.Format(
                        "Retrieve some user info from the RTVision Api, but we're missing one or both: Id: '{0}' and Login: '{1}'.",
                        string.IsNullOrEmpty(response.Data.Id.ToString()) ? "--missing--" : response.Data.Id.ToString(),
                        string.IsNullOrEmpty(response.Data.Login) ? "--missing--" : response.Data.Login));
            }

            return new UserInformation
            {
                Id = response.Data.Id.ToString(),
                Name = response.Data.Name,
                Email = response.Data.Email ?? "",
                UserName = response.Data.Login
            };
        }
        */

        #endregion
    }
}
