using Jose;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace UPP.Security
{
    public static class IdentityProvider
    {
        public static JwtSettings JwtSettings = new JwtSettings
        {
            JsonMapper = new JoseNewtonsoftMapper()
        };
    }

    public abstract class IdentityProvider<TContext, TUser> where TUser : class
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly AuthSettings _authSettings;
        private const string _bearerDeclaration = "Bearer ";

        public IdentityProvider(AuthSettings authSettings)
        {
            _authSettings = authSettings;
        }

        // Subclasses need to implement how to extract a token from specific context
        public abstract string GetToken(TContext context);
        public abstract TUser CreateUser(AuthToken token);

        public TUser GetUserIdentity(TContext context)
        {
            try
            {
                var jwt = GetToken(context);

                if (String.IsNullOrWhiteSpace(jwt))
                {
                    return null;
                }

                var authToken = Jose.JWT.Decode<AuthToken>(jwt, _authSettings.SecretKey, JwsAlgorithm.HS256, IdentityProvider.JwtSettings);
                if (authToken.Exp < DateTime.UtcNow)
                {
                    logger.Debug("Token is expired: {0}", authToken.Exp);
                    return null;
                }

                // Take the claims in the token a hydrate an AuthUser object with all the relevant user
                // information
                logger.Debug("Hydrating user information from token");
                return CreateUser(authToken);
            }
            catch (Exception e)
            {
                logger.Warn(e);
                return null;
            }
        }
    }
}
