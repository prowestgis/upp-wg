using Jose;
using Manager.Configuration;
using Nancy;
using Nancy.Security;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Manager.Security
{
    public interface IIdentityProvider
    {
        IUserIdentity GetUserIdentity(NancyContext context);
    }

    internal sealed class UPPIdentityProvider : IIdentityProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AuthSettings _authSettings;
        private const string _bearerDeclaration = "Bearer ";

        public static JwtSettings JwtSettings = new JwtSettings
        {
            JsonMapper = new NewtonsoftMapper()
        };

        private class NewtonsoftMapper : IJsonMapper
        {
            public T Parse<T>(string json)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }

            public string Serialize(object obj)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
        }
        public UPPIdentityProvider(AuthSettings authSettings)
        {
            _authSettings = authSettings;
        }

        public IUserIdentity GetUserIdentity(NancyContext context)
        {
            try
            {
                var authorizationCookie = context.Request.Cookies.ContainsKey(_authSettings.CookieName) ?
                    context.Request.Cookies[_authSettings.CookieName] :
                    null;

                var authorizationHeader = context.Request.Headers.Authorization;

                // Prefer to use the header (so it's last)
                string jwt = null;
                if (!String.IsNullOrEmpty(authorizationCookie))
                {
                    jwt = authorizationCookie;
                    logger.Debug("Getting JWT from cookie: {0}", jwt);
                }

                if (!String.IsNullOrEmpty(authorizationHeader))
                {
                    jwt = authorizationHeader.Substring(_bearerDeclaration.Length);
                    logger.Debug("Getting JWT from header: {0}", jwt);
                }

                if (String.IsNullOrWhiteSpace(jwt))
                {
                    return null;
                }

                var authToken = Jose.JWT.Decode<AuthToken>(jwt, _authSettings.SecretKey, JwsAlgorithm.HS256, UPPIdentityProvider.JwtSettings);
                if (authToken.Exp < DateTime.UtcNow)
                {
                    logger.Debug("Token is expired: {0}", authToken.Exp);
                    return null;
                }

                // Take the claims in the token a hydrate an AuthUser object with all the relevant user
                // information
                logger.Debug("Hydrating user information from token");
                var user = new AuthUser(authToken);

                // For development, all users are treated as haulers
                user.AddClaim("hauler", "*");                

                return user;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                return null;
            }
        }
    }

    /// <summary>
    /// Mediate access to secret keys
    /// </summary>
    public class AuthSettings
    {
        private static string secretKey = ConfigurationManager.AppSettings[AppKeys.JWT_SIGNING_KEY];

        public AuthSettings()
        {
            SecretKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
        }

        public string SecretKeyBase64 { get; set; }
        public string CookieName => "uppToken";

        public byte[] SecretKey
        {
            get
            {
                return Convert.FromBase64String(SecretKeyBase64);
            }
        }
    }

    public class AuthUser : IUserIdentity
    {
        // Metainformation
        public string AuthenticationProvider { get; }
        public bool IsAuthenticated { get; }
        public DateTime UtcExpiration { get; }

        // Core claims
        public string Name { get; }
        public string Email { get; }
        public string Phone { get; }

        // Required by IUserIdentity
        public string UserName { get; }
        public IEnumerable<string> Claims { get { return ExtendedClaims.Keys; } }

        // Better way to check claims
        public IDictionary<string, object> ExtendedClaims { get; }

        public AuthUser()
        {
            IsAuthenticated = false;
            ExtendedClaims = new Dictionary<string, object>();
        }

        public AuthUser(AuthToken token) : this()
        {
            AuthenticationProvider = token.Iss;
            IsAuthenticated = true;

            Name = token.Sub;
            UserName = token.Sub;
            Email = token.Email;
            Phone = token.Phone;

            // Copy the claims
            ExtendedClaims.Add("iss", token.Iss);
            ExtendedClaims.Add("idp", token.Idp);
            ExtendedClaims.Add("sub", token.Sub);
            ExtendedClaims.Add("exp", token.Exp);
            ExtendedClaims.Add("upp", token.Upp);
            ExtendedClaims.Add("email", token.Email);
            ExtendedClaims.Add("phone", token.Phone);

            UtcExpiration = token.Exp;
        }

        public void AddClaim(string claim, string value)
        {
            ExtendedClaims.Add(claim, value);
        }
    }
}
