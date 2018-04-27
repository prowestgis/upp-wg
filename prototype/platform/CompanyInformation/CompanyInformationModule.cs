using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;
using UPP.Security;

namespace CompanyInformation
{
    public sealed class MainViewModel
    {
        public HostConfigurationSection Configuration { get; set; }
    }

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

    public sealed class WebModule : NancyModule
    {
        public WebModule(HostConfigurationSection config, Database database)
        {
            // Need a valid JWT to access
            //this.RequiresAuthentication();
            //this.RequiresClaims(new[] { Claims.UPP_ADMIN });

            // Provide a view for configuring the issuer's behavior
            Get["/"] = _ => View["Index", new MainViewModel { Configuration = config }];
            Get["/data/query"] = _ => Response.AsJson(QueryDatabase(database));
            Get["/users/query"] = _ => Response.AsJson(QueryUsers(database));
        }

        private object QueryUsers(Database database)
        {
            var qs = Context.Request.Query;

            int start = qs.start;
            int length = qs.length;

            return Query(database, database.Users(start, length).Select(x => new object[]
                {
                    x.Email,
                    x.Companies
                })
            );
        }

        private object QueryDatabase(Database database)
        {
            var qs = Context.Request.Query;

            int start = qs.start;
            int length = qs.length;

            return Query(database, database.Query(start, length).Select(x => new object[]
                {
                    x.CompanyName,
                    x.Email,
                    x.Contact,
                    x.Phone,
                    x.Fax,
                    x.Cell,
                    x.BillTo,
                    x.BillingAddress
                })
            );
        }

        private object Query(Database database, IEnumerable<object> items)
        {
            // Fetch the passed parameters
            var qs = Context.Request.Query;

            int draw = qs.draw;
            int start = qs.start;
            int length = qs.length;

            int total = database.Count();
            int filtered = total;

            return new
            {
                Draw = draw,
                RecordsTotal = total,
                RecordsFiltered = filtered,
                Data = items
            };
        }

    }
}