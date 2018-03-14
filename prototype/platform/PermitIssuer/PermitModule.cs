using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;
using UPP.Protocols;

namespace PermitIssuer
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class PermitModule : NancyModule
    {
        public PermitModule(HostConfigurationSection config) : base("/api/v1/issue")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Post["/"] = _ => ProcessApplication(config);
        }

        private Response ProcessApplication(HostConfigurationSection config)
        {
            var identifier = config.Keyword(Keys.SELF__IDENTIFIER);

            return Response.AsJson(new PermitApprovalRecord
            {
                Timestamp = DateTime.Now,
                Status = PermitApprovalStatus.APPROVED,
                Identifier = identifier
            });
        }
    }
}
