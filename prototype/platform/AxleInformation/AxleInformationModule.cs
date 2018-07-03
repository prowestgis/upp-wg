using Nancy;
using Nancy.Security;
using UPP.Security;
using UPP.Common;

namespace AxleInformation
{    
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class AxleInformationModule : NancyModule
    {
        public AxleInformationModule(Database database) : base("/api/v1/axles")
        {
            // Allow from localhost unconditionally
            this.AllowFromLocalHost();

            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Allow cross-origin requests
            this.EnableCORS();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJsonAPI(database.FindAxlesInfoForUser(Context.CurrentUser));
        }
    }
}
