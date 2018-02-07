using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class CompanyInformationModule : NancyModule
    {
        public CompanyInformationModule(Database database) : base("/api/v1/companies")
        {
            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJson(database.FindCompanyInfoForUser(Context.CurrentUser));
        }
    }
}
