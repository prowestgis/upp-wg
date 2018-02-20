using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class TrailerInformationModule : NancyModule
    {
        public TrailerInformationModule(Database database) : base("/api/v1/trucks")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJson(database.FindTrucksInfoForUser(Context.CurrentUser));
        }
    }
}
