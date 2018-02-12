using Nancy;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace UPP.Common
{
    /// <summary>
    /// Tie into the Nancy pipeline and try to register the current service with the
    /// UPP service directory
    /// </summary>
    public static class RegisterWithServiceDirectory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Number of requests
        private static int requestCount = 0;

        // Attempt to register ourselves after this many requests come in
        private const int REQUEST_THRESHOLD = 10;

        public static Func<NancyContext, Response> GetPipelineHook(string hostIdentity, HostConfigurationSection config)
        {
            var baseUrl = config.Keyword(Keys.SERVICE_DIRECTORY__BASE_URI);
            return ctx =>
            {
                // Increment the request counter
                requestCount += 1;

                // Have we exceeded the number of requests?
                if (requestCount > REQUEST_THRESHOLD)
                {
                    // Reset the counter
                    requestCount = 0;

                    // Try to POST our registration information to the service directory
                    var client = new RestClient(baseUrl);
                    var request = new RestRequest("api/v1/agent/register", Method.POST);

                    // This is the URI that should be used to access the services
                    request.AddParameter("uri", config.Keyword(Keys.SERVICE_DIRECTORY__HOST_URI));
                    
                    // These are the UPP scopes that this implementation provides access to
                    request.AddParameter("scopes", config.Keyword(Keys.SERVICE_DIRECTORY__SCOPES));

                    // Certificate / Key that identifies us as a whitelisted entity
                    request.AddParameter("whoami", hostIdentity);

                    // We provide UPP services
                    request.AddParameter("type", "upp");

                    logger.Debug("Attempting to register with service directory at {0}", request.Resource);

                    var response = client.Execute(request);
                }

                // Never stop the pipeline
                return null;
            };
        }
    }
}
