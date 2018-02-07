using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
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

    public class NancyAuthUser : AuthUser, IUserIdentity
    {
        public NancyAuthUser(AuthToken token) : base(token) { }
    }

    public class NancyIdentityProvider : IdentityProvider<NancyContext, IUserIdentity>, IIdentityProvider
    {
        public NancyIdentityProvider(AuthSettings authSettings)
            : base(authSettings)
        {
        }

        public override string GetToken(NancyContext context)
        {
            // Get the token from either the special cookie or the Authorization header
            return context.Request.Cookies.ContainsKey(_authSettings.CookieName)
                ? context.Request.Cookies[_authSettings.CookieName]
                : context.Request.Headers.Authorization;
        }

        public override IUserIdentity CreateUser(AuthToken token)
        {
            var user = new NancyAuthUser(token);

            // For development, all users are treated as haulers
            user.AddClaim("hauler", "*");

            return user;
        }
    }
}
