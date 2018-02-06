using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDirectory
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class ServiceDiscoveryAgent : NancyModule
    {
        public ServiceDiscoveryAgent() : base("/api/v1/agent")
        {
            //this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Get["/register"] = _ => Response.AsJson(new { Message = "Hello" });
        }
    }

    /// <summary>
    /// The basic service discovery API allows trusted clients to query for hosts that
    /// provide core UPP services
    /// </summary>
    public sealed class ServiceDiscoveryHosts : NancyModule
    {
        public ServiceDiscoveryHosts(Database database) : base("/api/v1/hosts")
        {
            Get["/"] = _ => Response.AsJson(database.MicroServiceProviders.ToList());
        }
    }
}
