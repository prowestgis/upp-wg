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

namespace Manager.Host
{
    public sealed class Frontend : NancyModule
    {
        public Frontend(Services services)
        {
            Get["/"] = _ => View["Index", new DashboardView(services, Context)];
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
