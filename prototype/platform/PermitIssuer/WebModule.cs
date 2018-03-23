using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace PermitIssuer
{
    public sealed class WebModule : NancyModule
    {
        public WebModule(HostConfigurationSection config)
        {
            // Need a valid JWT to access
            // this.RequiresAuthentication();

            // Provide a view for configuring the issuer's behavior
            Get["/"] = _ => View["Index", new { Configuration = config }];
        }
    }
}