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
        public static void AllowFromLocalHost(this INancyModule module, IUserIdentity user)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.CheckIsLocal(user), "Allow from localhost");
        }

        private static Func<NancyContext, Response> CheckIsLocal(IUserIdentity user)
        {
            return (ctx) =>
            {
                if (ctx.Request.IsLocal() && !ctx.CurrentUser.IsAuthenticated())
                {
                    ctx.CurrentUser = user;
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
        private static IUserIdentity localUser = new NancyAuthUser(new AuthToken() { Sub = "local", Email = "nobody@example.com" });

        public AxleInformationModule(Database database) : base("/api/v1/axles")
        {
            // Allow from localhost unconditionally
            this.AllowFromLocalHost(localUser);

            // Need a valid JWT to access
            this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Get["/"] = _ => Response.AsJsonAPI(database.FindAxlesInfoForUser(Context.CurrentUser));
        }
    }
}
