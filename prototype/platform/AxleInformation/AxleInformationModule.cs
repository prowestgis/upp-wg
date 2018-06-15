using Nancy;
using Nancy.Security;
using Nancy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UPP.Security;
using UPP.Common;

namespace AxleInformation
{
    public static class SecurityHooks
    {
        public static void AllowFromLocalHost(this INancyModule module)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.CheckIsLocal(), "Allow from localhost");
        }

        private static Func<NancyContext, Response> CheckIsLocal()
        {
            return (ctx) =>
            {
                if (ctx.Request.IsLocal() && !ctx.CurrentUser.IsAuthenticated())
                {
                    ctx.CurrentUser = new NancyAuthUser(new AuthToken() { Sub = "local", Email = "nobody@example.com" });
                }

                return null;
            };
        }
    }
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

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJson(database.FindAxlesInfoForUser(Context.CurrentUser));
        }
    }
}
