using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
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
            var record = this.Bind<ServiceRegistrationRecord>();

            // Attempt to register with the services table
            logger.Debug("  Uri    = {0}", record.Uri);
            logger.Debug("  Type   = {0}", record.Type);
            logger.Debug("  Whoami = {0}", record.Whoami);
            logger.Debug("  Scopes = {0}", record.Scopes);

            try
            {
                var response = database.RegisterService(record);

                // Return the response as a payload
                return Response.AsJson(response);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Response.AsJson(new { Success = false });
            }
        }
    }

    /// <summary>
    /// The basic service discovery API allows trusted clients to query for hosts that
    /// provide core UPP services
    /// </summary>
    public sealed class ServiceDiscoveryHosts : NancyModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ServiceDiscoveryHosts(Database database) : base("/api/v1/hosts")
        {
            this.RequiresAuthentication();
            // this.RequiresClaims(new[] { });

            Get["default", "/"] = _ => ListHosts(database, Request.Query["type"]);
            Get["get_service", "/{name}/access"] = _ => AccessHost(database, _.name);
        }

        private Response ListHosts(Database database, string type)
        {
            return Response.AsJson(database.FindMicroServiceProviderByType(type).ToList());
        }

        private Response AccessHost(Database database, string name)
        {
            // See if the service exists in the collection of microservices
            var service = database.FindMicroServiceProviderByName(name);
            
            // Service not found, return a 404
            if (service == null)
            {
                logger.Debug("No route microservice provider found for {0}", name);
                return Response.AsJson(Json404.Default, HttpStatusCode.NotFound);
            }

            // See if the service is secured with OAuth
            var oauth = database.FindOAuthProviderForService(service);

            // If the service requires an access_token, try to acquire it now. We currently
            // only support the client_credentials grant type.
            //
            // https://tools.ietf.org/html/rfc6749#section-4.4
            if (oauth != null)
            {
                logger.Debug("Fetching token for {0}", oauth.Name);
                var token = UPP.Security.OAuth.GetToken(oauth);
                logger.Debug("Got token {0}", token);
                return Response.AsJson(new ServiceAccessRecord(service, token));
            }

            return Response.AsJson(new ServiceAccessRecord(service));
        }
    }

    public sealed class ServiceAccessRecord
    {
        public ServiceAccessRecord(MicroServiceProviderConfig service)
            : this(service, null, false)
        {
        }

        public ServiceAccessRecord(MicroServiceProviderConfig service, string token)
            : this(service, token, true)
        {
        }

        private ServiceAccessRecord(MicroServiceProviderConfig service, string token, bool secured)
        {
            Name = service.Name;
            Url = service.Uri;
            Token = token;
            IsSecured = secured;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Token { get; private set; }
        public bool IsSecured { get; private set; }
    }
}
