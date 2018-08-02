using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Common;

namespace InsuranceInformation
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class InsuranceInformationModule : NancyModule
    {
        public InsuranceInformationModule(Database database) : base("/api/v1/insurers")
        {
            // Allow from localhost unconditionally
            this.AllowFromLocalHost();

            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Allow cross-origin requests
            this.EnableCORS();

            // Registers service metadata from a trusted source
            Get["/"] = _ => GetInsurers(database);
        }

        private Response GetInsurers(Database database)
        {
            var model = database.FindInsuranceInfoForUser(Context.CurrentUser).Select(x => new AttributeResourceObject
            {
                Id = x.Id,
                Type = x.Type,
                Attributes = new
                {
                    x.AgencyAddress,
                    x.InsuredAmount,
                    x.PolicyNumber,
                    x.ProviderName
                }
            });

            return Response.AsJsonAPI(model);
        }
    }
}
