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

namespace Manager.Security
{
    public interface IIdentityProvider
    {
        IUserIdentity GetUserIdentity(NancyContext context);
    }

    internal sealed class UPPIdentityProvider : IIdentityProvider
    {
        private readonly AuthSettings _authSettings;
        private const string _bearerDeclaration = "Bearer ";

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
                }

                if (!String.IsNullOrEmpty(authorizationHeader))
                {
                    jwt = authorizationHeader.Substring(_bearerDeclaration.Length);
                }

                var authToken = Jose.JWT.Decode<AuthToken>(jwt, _authSettings.SecretKey, JwsAlgorithm.HS256);
                if (authToken.Exp < DateTime.UtcNow)
                    return null;

                return new AuthUser(authToken.Name, authToken.Sub, authToken.UserId);
            }
            catch (Exception)
            {
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
        public string AuthenticationType => "Test";
        public bool IsAuthenticated { get; }
        public string Name { get; }
        public Guid Id { get; }
        public string UserName { get; }
        public IEnumerable<string> Claims { get; }

        public AuthUser(string name, string login, Guid id)
        {
            Name = name;
            UserName = login;
            Id = id;
            IsAuthenticated = true;            
            Claims = new string[] { };
        }
    }
}
