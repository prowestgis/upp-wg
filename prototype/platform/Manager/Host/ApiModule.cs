using Nancy;
using Nancy.Responses;
using Nancy.Security;
using Nancy.Authentication.Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
