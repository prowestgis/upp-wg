using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;
using UPP.Security;

namespace UPP.Common
{
    // IUserIdentity helpers wrt Claims
    public static class UserIdentityHelpers
    {
        public static IEnumerable<string> EmailAddresses(this IUserIdentity identity)
        {
            var user = identity as NancyAuthUser;
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            // Assume no commas in the email address
            return user.Email.Split(',').Select(x => x.Trim());
        }
    }

    public interface IIdentityProvider
    {
        IUserIdentity GetUserIdentity(NancyContext context);
    }

    public interface IAdditionalClaimProvider
    {
        IDictionary<string, object> FindClaims(AuthToken token);
    }

    public sealed class DefaultAdditionalClaimProvider : IAdditionalClaimProvider
    {
        private static IDictionary<string, object> EMPTY_DICTIONARY = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());

        public IDictionary<string, object> FindClaims(AuthToken token)
        {
            return EMPTY_DICTIONARY;
        }
    }

    public class NancyAuthUser : AuthUser, IUserIdentity
    {
        public NancyAuthUser(AuthToken token) : base(token) { }
    }

    public class NancyIdentityProvider : IdentityProvider<NancyContext, IUserIdentity>, IIdentityProvider
    {
        private IAdditionalClaimProvider _provider;

        public NancyIdentityProvider(AuthSettings authSettings, IAdditionalClaimProvider claimProvider)
            : base(authSettings)
        {
            _provider = claimProvider;
        }

        public override string GetToken(NancyContext context)
        {
            // Get the token from either the special cookie or the Authorization header
            if (context.Request.Cookies.ContainsKey(_authSettings.CookieName))
            {
                return context.Request.Cookies[_authSettings.CookieName];
            }

            if (!String.IsNullOrEmpty(context.Request.Headers.Authorization))
            {
                var token = context.Request.Headers.Authorization.Replace("Bearer", "");
                token = token.Trim();
                return token;
            }

            return null;
        }

        public override IUserIdentity CreateUser(AuthToken token)
        {
            var user = new NancyAuthUser(token);

            // Lookup any extra claims that need to be added to this user
            if (_provider != null)
            {
                foreach (var claim in _provider.FindClaims(token))
                {
                    user.AddClaim(claim.Key, claim.Value);
                }
            }

            // For development, all users are treated as haulers
            user.AddClaim("hauler", "*");

            return user;
        }
    }
}
