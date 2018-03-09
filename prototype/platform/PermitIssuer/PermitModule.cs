using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitIssuer
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class PermitModule : NancyModule
    {
        public PermitModule() : base("/api/v1/issue")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Post["/"] = _ => ProcessApplication();
        }
        private Response ProcessApplication()
        {
            return Response.AsJson(new { Status = "Approved" });
        }
    }
}
