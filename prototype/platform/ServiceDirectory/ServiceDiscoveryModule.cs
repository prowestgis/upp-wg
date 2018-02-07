using Nancy;
using Nancy.ModelBinding;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Protocols;

namespace ServiceDirectory
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class ServiceDiscoveryAgent : NancyModule
    {
        private static BindingConfig DeafultBindingConfig = new BindingConfig() { BodyOnly = true };
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ServiceDiscoveryAgent(Database database) : base("/api/v1/agent")
        {
            //this.RequiresAuthentication();

            // Registers service metadata from a trusted source
            Post["/register"] = _ => RegisterService(database);
        }

        private Response RegisterService(Database database)
        {
            logger.Debug("Registering a new service");

            // Bind the request to the ServiceRegistrationRecord
            var record = this.Bind<ServiceRegistrationRecord>(DeafultBindingConfig);

            // Attempt to register with the services table
            logger.Debug("  Uri    = {0}", record.Uri);
            logger.Debug("  Type   = {0}", record.Type);
            logger.Debug("  Whoami = {0}", record.Whoami);
            logger.Debug("  Scopes = {0}", record.Scopes);

            var response = database.RegisterService(record);

            // Return the response as a payload
            return Response.AsJson(response);
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
