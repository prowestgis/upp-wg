using Manager.Store;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Security;

namespace Manager.Host
{
    public sealed class AccountModule : NancyModule
    {
        public AccountModule(Services services) : base("/account")
        {
            this.RequiresAuthentication();

            Get["/link"] = _ => LinkAccount();
            Post["/link"] = _ => DoLinkAccount(services);
        }

        public Response DoLinkAccount(Services services)
        {
            // Get the current user's UPP GUID
            var user = Context.CurrentUser as AuthUser;

            // Stick them into the database
            services.AddToIdentityFromExternalAuth(user.ExtendedClaims["upp"].ToString(), "", "");

            // Update their claims and return a new cookie

            return Response.AsRedirect("/manager");
        }

        public Negotiator LinkAccount()
        {
            var model = new AcountLinkModel
            {
                Idp = Request.Query["idp"],
                Sub = Request.Query["sub"],
                Email = Request.Query["email"]
            };

            // Ask the user if they want to link the current OAuth provider to their existing UPP account
            return View["Link", model];
        }
    }

    public sealed class AcountLinkModel
    {
        public string Idp { get; set; }
        public string Sub { get; set; }
        public string Email { get; set; }
    }
}
