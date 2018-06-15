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
using Newtonsoft.Json.Schema;

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

            Get["register/schema"] = _ => Response.AsJson(ServiceRegistrationRequest.JsonSchema());
        }

        private Response RegisterService(Database database)
        {
            logger.Debug("Registering a new service");

            // Bind the request to the ServiceRegistrationRecord
            var record = this.Bind<ServiceRegistrationRequest>();

            // Attempt to register with the services table
            logger.Debug("  Uri    = {0}", record.Spec.Path);
            logger.Debug("  Type   = {0}", record.MetaData.Labels.Type);
            logger.Debug("  Whoami = {0}", record.MetaData.Whoami);
            logger.Debug("  Scopes = {0}", record.MetaData.Labels.Scopes);

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

            Get["default", "/"] = _ => ListHosts(database, Request.Query["type"], Request.Query["scope"], Request.Query["authority"]);
            Get["get_service", "/{name}/access"] = _ => AccessHost(database, _.name);
            Get["schema/access"] = _ => Response.AsJson(ServiceAccessRecord.JsonSchema());
        }

        private Response ListHosts(Database database, string type, string scope, string authority)
        {
            return Response.AsJson(database.FindMicroServiceProviderByType(type).Where(x => scope == null || x.Scopes.Contains(scope)).ToList());
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
                var oauth_token = UPP.Security.OAuth.GetToken(oauth);
                logger.Debug("Got token {0}", oauth_token);
                return Response.AsJson(new ServiceAccessRecord(service, oauth_token));
            }

            // See if the service is secured with a regular token endpoint
            var token = database.FindTokenProviderForService(service);
            if (token != null)
            {
                logger.Debug("Fetching token for {0}", token.Name);
                var web_token = UPP.Security.WebToken.GetToken(token);
                logger.Debug("Got token {0}", web_token);
                return Response.AsJson(new ServiceAccessRecord(service, web_token));
            }

            return Response.AsJson(new ServiceAccessRecord(service));
        }
    }
}
