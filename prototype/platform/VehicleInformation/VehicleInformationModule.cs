using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Common;

namespace VehicleInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class VehicleInformationModule : NancyModule
    {
        public VehicleInformationModule(Database database) : base("/api/v1/vehicles")
        {
            // Allow from localhost unconditionally
            this.AllowFromLocalHost();

            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Allow cross-origin requests
            this.EnableCORS();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJsonAPI(database.FindVehiclesInfoForUser(Context.CurrentUser));
        }
    }
}
