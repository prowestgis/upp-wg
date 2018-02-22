using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxleInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class AxleInformationModule : NancyModule
    {
        public AxleInformationModule(Database database) : base("/api/v1/axles")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJson(database.FindAxlesInfoForUser(Context.CurrentUser));
        }
    }
}
