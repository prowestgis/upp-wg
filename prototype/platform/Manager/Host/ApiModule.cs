using Nancy;
using Nancy.Responses;
using Nancy.Security;
using Nancy.Authentication.Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.API;
using Newtonsoft.Json;
using Manager.Security;
using Manager.Store;

namespace Manager.Host
{
    /// <summary>
    /// Top-level module that implements all of the specification's API.  This could evenually be linked
    /// to an auto-generated backend.  The API is secure and requires authentication.
    /// </summary>
    public sealed class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            //this.RequiresAuthentication();

            Get["/"] = _ => Response.AsJson(new { Message = "Hello", User = Context.CurrentUser });
        }
    }

    /// <summary>
    /// Stub for asking about the company information for a UPP Identity  
    /// </summary>
    public sealed class CompanyApi : NancyModule
    {
        public CompanyApi(Services services) : base("/api/ext/company")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { Claims.HAULER });

            Get["/"] = _ =>
            {
                // The user should have an extended claim with their UPP identity.  Use this and then
                // look up the company information associated with this user
                var upp_id = (Context.CurrentUser as AuthUser).ExtendedClaims["upp"].ToString();
                return Response.AsJson(services.FindCompanyInfoForUser(upp_id));
            };
        }
    }

    /// <summary>
    /// Implement functionality associated with the Hauler Info scope.  
    /// </summary>
    public sealed class HaulerApiInfo : NancyModule
    {
        public HaulerApiInfo() : base("/api/hauler")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { Claims.HAULER });

            Get["/"] = _ => Response.AsJson(new HaulerInfoView(Context));
        }
    }

    public sealed class HaulerInfoView
    {
        public HaulerInfoView(NancyContext context)
        {
            // Need some of our specific methods
            var user = context.CurrentUser as AuthUser;

            if (user == null)
            {
                throw new ArgumentNullException();
            }

            ApplicantName = user.Name;
            ApplicationDate = DateTime.Now;
            ApplicantEmail = user.Email;
            ApplicantPhone = user.Phone;
            ApplicantFax = null;
        }

        // Fields defined in UPP committee specificiation
        public string ApplicantName { get; private set; }
        public DateTime ApplicationDate { get; private set; }
        public string ApplicantEmail { get; private set; }
        public string ApplicantPhone { get; private set; }
        public string ApplicantFax { get; private set; }
    }
}
