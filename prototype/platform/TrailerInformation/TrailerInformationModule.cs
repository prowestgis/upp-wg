using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Common;

namespace TrailerInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class TrailerInformationModule : NancyModule
    {
        public TrailerInformationModule(Database database) : base("/api/v1/trailers")
        {
            // Allow from localhost unconditionally
            this.AllowFromLocalHost();

            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Allow cross-origin requests
            this.EnableCORS();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJsonAPI(database.FindTrailersInfoForUser(Context.CurrentUser));
        }
    }
}
