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
        private Func<NancyContext, string> EvaluatePermit = DefaultPermitEvaluation;
        private static Random RNG = new Random();

        public PermitModule(HostConfigurationSection config) : base("/api/v1/issue")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Post["/"] = _ => ProcessApplication(config);
        }

        private static string DefaultPermitEvaluation(NancyContext context)
        {
            switch (WebModule.Configuration.Behavior)
            {
                case "always": return PermitApprovalStatus.APPROVED;
                case "random": return (RNG.NextDouble() < 0.5) ? PermitApprovalStatus.APPROVED : PermitApprovalStatus.DENIED;
                default:
                case "never": return PermitApprovalStatus.DENIED;
            }
        }

        private Response ProcessApplication(HostConfigurationSection config)
        {
            var identifier = config.Keyword(Keys.SELF__IDENTIFIER);
            var status = EvaluatePermit(Context);

            return Response.AsJson(new PermitApprovalRecord
            {
                Timestamp = DateTime.Now,
                Status = status,
                Identifier = identifier
            });
        }
    }
}
