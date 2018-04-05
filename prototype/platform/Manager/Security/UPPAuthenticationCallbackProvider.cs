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
using UPP.Configuration;
using UPP.Security;

namespace Manager.Security
{
    internal sealed class UPPAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AuthSettings _authSettings;
        private readonly Services _services;
        private readonly string _baseUrl;
        private readonly List<string> _administrators;
            
        private const string _bearerDeclaration = "Bearer ";        

        public UPPAuthenticationCallbackProvider(AuthSettings authSettings, Services services, HostConfigurationSection config)
        {
            _authSettings = authSettings;
            _services = services;
            _baseUrl = config.Keyword(Keys.NANCY__HOST_BASE_URI) ?? "/";
            _administrators = config.Keyword(Keys.UPP__ADMINISTRATORS).Split(';').Select(x => x.Trim()).ToList();
        }

        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            logger.Debug("Received callback from provider: '{0}'", model.ProviderName);

            if (model.Exception != null)
            {
                logger.Warn(model.Exception);
                throw model.Exception;
            }

            try
            {
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
                var currentUser = nancyModule.Context.CurrentUser as AuthUser;

                // Case 1: No record in database, not yet logged in
                if (existingUser == null && currentUser == null)
                {
                    existingUser = _services.CreateNewIdentityFromExternalAuth(model.ProviderName, model.AuthenticatedClient.UserInformation.Email, model.AuthenticatedClient.UserInformation.Id);
                }

                // Case 2: No record in database, use is currently logged in unde different identity
                if (existingUser == null && currentUser != null)
                {
                    existingUser = currentUser.ExtendedClaims["upp"].ToString();
                    _services.AddToIdentityFromExternalAuth(existingUser, model.ProviderName, model.AuthenticatedClient.UserInformation.Id);
                }

                // If the current user is in the configured list of UPP administrators, add the appropriate claim to their record
                if (_administrators.Contains(model.AuthenticatedClient.UserInformation.Email))
                {
                    _services.AddClaimToIdentity(existingUser, Claims.UPP_ADMIN);
                }

                // Create an encrypted JWT from the user information with appropriate claims
                var req_url = nancyModule.Context.Request.Url;
                var iss = String.Format("{0}://{1}:{2}", req_url.Scheme, req_url.HostName, req_url.Port);

                // Now, what to do about the token.  If the user is not currently logged in, just create a new token.  Otherwise,
                // extend the existing claims

                var tokenPayload = new AuthToken
                {
                    Iss = iss,
                    Idp = model.ProviderName,
                    Sub = model.AuthenticatedClient.UserInformation.UserName,
                    Exp = DateTime.UtcNow.AddHours(1),
                    Email = model.AuthenticatedClient.UserInformation.Email,
                    Upp = existingUser,
                    Phone = "1-800-867-5309"
                };

                if (currentUser != null)
                {
                    tokenPayload.Idp = String.Format("{0} {1}", currentUser.ExtendedClaims["idp"], tokenPayload.Idp);
                    tokenPayload.Email = String.Format("{0} {1}", currentUser.ExtendedClaims["email"], tokenPayload.Email);
                }

                var token = Jose.JWT.Encode(tokenPayload, _authSettings.SecretKey, JwsAlgorithm.HS256, null, IdentityProvider.JwtSettings);
                var cookie = new NancyCookie(_authSettings.CookieName, token);

                logger.Debug("Setting JWT cookie: {0} = {1}", _authSettings.CookieName, token);

                return nancyModule.Response
                    .AsRedirect(_baseUrl)
                    .WithCookie(cookie);
            }
            catch (Exception e)
            {
                logger.Debug(e);
                return nancyModule.Response.AsRedirect(_baseUrl);
            }
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            throw new System.NotImplementedException(); // Provider canceled auth or it failed for some reason e. g. user canceled it
        }
    }
}
