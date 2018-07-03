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
using UPP.Common;
using Newtonsoft.Json;
using System.IO;

namespace ServiceDirectory
{
    /// <summary>
    /// The basic service discovery API allows trusted clients to query for hosts that
    /// provide core UPP services
    /// </summary>
    public sealed class ServiceDiscoveryHosts : NancyModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ServiceDiscoveryHosts(Database database) : base("/api/v1/services")
        {
            // this.RequiresAuthentication();
            // this.RequiresClaims(new[] { });

            Get["/"] = _ => ListHosts(database);
            Post["/"] = _ => RegisterService(database);

            // Get service token API
            Get["/{name}/token"] = _ => AccessHost(database, _.name);
            Post["/{name}/token"] = _ => AccessHost(database, _.name);
        }

        private Response ListHosts(Database database)
        {
            // Get the parameters from the query string
            var type = ((string) Request.Query["type"]) ?? "all";
            var scope = ((string) Request.Query["scope"]) ?? "all";
            var authority = ((string) Request.Query["authority"]) ?? "all";
            string sort = ((string)Request.Query["sort"]) ?? "name";
            var direction = ((string) Request.Query["direction"]) ?? "desc";

            // Map the sort direction onto a real property name
            switch (sort.ToLower())
            {
                default:
                case "name":
                    sort = Helpers.GetMemberInfo((MicroServiceProviderConfig _) => _.Name).Name;
                    break;

                case "type":
                    sort = Helpers.GetMemberInfo((MicroServiceProviderConfig _) => _.Type).Name;
                    break;

                case "display_name":
                    sort = Helpers.GetMemberInfo((MicroServiceProviderConfig _) => _.DisplayName).Name;
                    break;
            }

            // Perform the query
            var filter = database.FindMicroServiceProviderByType(type)
                .AsQueryable()
                .Where(x => scope == "all" || x.Scopes.Contains(scope))
                .Where(x => authority == "all" || x.Authority == authority)
                .OrderBy(sort, direction == "asc")
                ;

            // Convert the result into a JSON API object
            var json = filter.Select(x => new _ServiceConfigJSAPI
            {
                Type = "microservice-config",
                Id = String.Format("{0}+{1}", x.Authority, x.Type),
                Attributes = new
                {
                    x.Authority,
                    x.Description,
                    x.DisplayName,
                    x.Format,
                    x.Name,
                    x.OAuthId,
                    x.Priority,
                    x.Scopes,
                    x.TokenId,
                    x.Type,
                    x.Uri,
                }
            });

            return Response.AsJsonAPI(json.AsEnumerable());
        }

        private class _ServiceConfigJSAPI : IResourceObject
        {
            public object Id { get; set; }
            public string Type { get; set; }
            public object Attributes { get; set; }
        }

        private Response RegisterService(Database database)
        {
            logger.Debug("Registering a new service");

            // Deserialize from the body
            using (var sr = new StreamReader(Request.Body))
            using (var reader = new JsonTextReader(sr))
            {
                try
                {
                    // Create our own serializer
                    var serializer = new Newtonsoft.Json.JsonSerializer();

                    // read the json from a stream
                    var record = serializer.Deserialize<ServiceRegistrationRecord>(reader);

                    // Register with the database
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
