using Jose;
using Manager.Store;
using Nancy;
using Nancy.Cookies;
using Nancy.SimpleAuthentication;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Security
{
    internal sealed class UPPAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AuthSettings _authSettings;
        private readonly Services _services;
        private const string _bearerDeclaration = "Bearer ";

        public UPPAuthenticationCallbackProvider(AuthSettings authSettings, Services services)
        {
            _authSettings = authSettings;
            _services = services;
        }
        
        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            logger.Debug("Received callback from provider: '{0}'", model.ProviderName);

            // Exchange for our own token
            //
            // First, extract the user's name from the OAuth response
            //
            // For Google (https://developers.google.com/+/web/api/rest/latest/people#resource)
            //   UserName = Display Name
            //       Name = Name
            //       Id = Id
            //       Email = <first email from emails list>

            // Check to the External Login table and see if this user is known to us.  If so, we add their UPP Identity to 
            // the claims.
            //
            // If there is no entry in the external login table, if the user is currently logged in, ask if this new log in should
            // be added to their current UPP Identity, otherwise create a new Identity
            var existingUser = _services.FindExternalUser(model.ProviderName, model.AuthenticatedClient.UserInformation.Id);
            var linkAccounts = false;

            if (existingUser == null)
            {
                var currentUser = nancyModule.Context.CurrentUser as AuthUser;
                if (currentUser != null)
                {
                    // Redirect to a confirmation page about adding this login to the UPP Identity
                    linkAccounts = true;
                }
                else
                {
                    // Create a new Identity in the database
                    existingUser = _services.CreateNewIdentityFromExternalAuth(model.ProviderName, model.AuthenticatedClient.UserInformation.Id);

                    // Add to a couple of random companies
                    _services.AssignUserToCompanies(existingUser, new[] { 5, 15, 103 });
                }
            }

            // Make sure the (user, provider) exist in our database.  Need to include the provider
            // to avoid the possibility of a differnt user from a different identity provider getting
            // mapped to the same UPP user.
            // var user = FindUser(provider, username);

            // Create an encrypted JWT from the user information with appropriate claims
            var tokenPayload = new AuthToken
            {
                Iss = model.ProviderName,
                Sub = model.AuthenticatedClient.UserInformation.UserName,
                Exp = DateTime.UtcNow.AddHours(1),
                Email = model.AuthenticatedClient.UserInformation.Email,
                Upp = existingUser,
                Phone = "1-800-867-5309"
            };

            var token = Jose.JWT.Encode(tokenPayload, _authSettings.SecretKey, JwsAlgorithm.HS256);
            var cookie = new NancyCookie(_authSettings.CookieName, token);

            logger.Debug("Setting JWT cookie: {0} = {1}", _authSettings.CookieName, token);

            // If the user is logged in, but authenticated to a new provider, ask if they want to 
            // link this provider to their UPP Identity
            if (linkAccounts)
            {
                return nancyModule.Response
                    .AsRedirect("/account/link")
                    .WithCookie(cookie);
            }

            return nancyModule.Response
                .AsRedirect("/")
                .WithCookie(cookie);
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            throw new System.NotImplementedException(); // Provider canceled auth or it failed for some reason e. g. user canceled it
        }
    }
}
