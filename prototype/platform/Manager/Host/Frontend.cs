using Nancy;
using Dapper;
using Manager.Store;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Security;
using Nancy.Cookies;

namespace Manager.Host
{
    public sealed class Frontend : NancyModule
    {
        private readonly AuthSettings _authSettings;

        public Frontend(Services services, AuthSettings authSettings)
        {
            _authSettings = authSettings;

            Get["/"] = _ => View["Index", new DashboardView(services, Context)];
            Get["/authentication/logout"] = _ => Logout();
        }

        public Response Logout()
        {
            // Reset the token to an empty string and set an very old expiration date
            var cookie = new NancyCookie(_authSettings.CookieName, String.Empty, DateTime.MinValue);
            return Response.AsRedirect("/").WithCookie(cookie);
        }
    }

    public sealed class DashboardView
    {
        public DashboardView(Services services, NancyContext context)
        {
            Logins = services.AuthenticationProviders.Select(x => new PrimaryLogin
            {
                Name = x.Name,
                Url = String.Format("/authentication/redirect/{0}", x.Name),
                Image = String.Format("/images/{0}-login-button.png", x.Name)
            }).ToList();

            User = context.CurrentUser as AuthUser;
        }

        public AuthUser User { get; set; }
        public List<PrimaryLogin> Logins { get; set; }

        public sealed class PrimaryLogin
        {
            public string Url { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
        }
    }
}
