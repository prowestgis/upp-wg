using Jose;
using Nancy;
using Nancy.Cookies;
using Nancy.SimpleAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Security
{
    internal sealed class UPPAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        private readonly AuthSettings _authSettings;
        private const string _bearerDeclaration = "Bearer ";

        public UPPAuthenticationCallbackProvider(AuthSettings authSettings)
        {
            _authSettings = authSettings;
        }

        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            // Exchange for our own token
            //
            // First, extract the user's name from the OAuth response
            var userName = model.AuthenticatedClient.UserInformation.UserName;
            var name = model.AuthenticatedClient.UserInformation.Name;
            var provider = model.ProviderName;

            // Make sure the (user, provider) exist in our database.  Need to include the provider
            // to avoid the possibility of a differnt user from a different identify provider getting
            // mapped to the same UPP user.
            // var user = FindUser(provider, username);

            // Create an encrypted JWT from the user information 
            var tokenPayload = new AuthToken
            {
                Name = name,
                Sub = userName,
                Exp = DateTime.UtcNow.AddHours(1),
                Claims = new [] { "admin", "provider.routing", "provider.approval" }
            };

            var token = Jose.JWT.Encode(tokenPayload, _authSettings.SecretKey, JwsAlgorithm.HS256);
            var cookie = new NancyCookie(_authSettings.CookieName, token);

            return nancyModule.Response
                .AsRedirect("/")
                .WithCookie(cookie);

            /*
            return nancyModule.Negotiate
                .WithView("AuthCallback")
                .WithModel(model)
                .WithCookie(cookie);
            */
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            throw new System.NotImplementedException(); // Provider canceled auth or it failed for some reason e. g. user canceled it
        }
    }
}
