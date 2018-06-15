using Nancy;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Common.Utilities;
using UPP.Configuration;
using UPP.Protocols;

namespace UPP.Common
{
    /// <summary>
    /// Tie into the Nancy pipeline and try to register the current service with the
    /// UPP service directory
    /// 
    /// Logic is as follows:
    /// 
    ///  1. When the class is created, try to register
    ///  2. If registration fails, then retry every so ofter
    ///  3. If registration succeeds, then retry every so often in case the service directory restarted
    ///  
    ///  The time span for [2] should be must less than [3], e.g. 10 seconds vs an hour.
    /// </summary>
    public class RegisterWithServiceDirectory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Last time we tried to contact the Service Directory
        private DateTime lastRequest;

        // Last time the pipeline callback was invoked
        private DateTime lastCallback;

        // Do we think that we are currently registered with the Service Directory?
        private bool IsRegistered = false;

        private readonly string hostIdentity;
        private readonly HostConfigurationSection config;

        private readonly TimeSpan retry;
        private readonly TimeSpan ping;

        public RegisterWithServiceDirectory(string hostIdentity, HostConfigurationSection config)
            : this(hostIdentity, config, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(15))
        {
        }

        public RegisterWithServiceDirectory(string hostIdentity, HostConfigurationSection config, TimeSpan retry, TimeSpan ping)
        {
            lastRequest = DateTime.MinValue;
            lastCallback = DateTime.Now;

            this.hostIdentity = hostIdentity;
            this.config = config;
            this.retry = retry;
            this.ping = ping;
        }

        private void PipelineCallback()
        {
            // Update the timestamp
            lastCallback = DateTime.Now;

            // How long since we tried to contact the Service Directory?
            var delta = lastCallback - lastRequest;

            // If we *don't* think we're registered, use the retry delay
            // If we think we *are* registered, just keep a heartbeat alive
            if ((!IsRegistered && delta >= retry) || (IsRegistered && delta > ping))
            {
                IsRegistered = TryToRegister();
            }
        }

        private void InitialRegistration()
        {
            IsRegistered = TryToRegister();
        }

        private bool TryToRegister()
        {
            // Get the base url of the service directory host along with other parameters
            var baseUrl = config.Keyword(Keys.SERVICE_DIRECTORY__BASE_URI);
            var serviceUri = config.Keyword(Keys.SERVICE_DIRECTORY__HOST_URI);
            var scopes = config.Keyword(Keys.SERVICE_DIRECTORY__SCOPES);

            // Make sure there are actual values passed in
            if (String.IsNullOrEmpty(baseUrl))
            {
                logger.Warn("No service directory URL set in the configuration file. Keyword '{0}'", Keys.SERVICE_DIRECTORY__BASE_URI);
                return false;
            }

            if (String.IsNullOrEmpty(serviceUri))
            {
                logger.Warn("No service URI is set in the configuration file. Keyword '{0}'", Keys.SERVICE_DIRECTORY__HOST_URI);
                return false;
            }

            if (String.IsNullOrEmpty(scopes))
            {
                logger.Warn("No service scopes are set in the configuration file. Keyword '{0}'", Keys.SERVICE_DIRECTORY__SCOPES);
                return false;
            }

            // Try to POST our registration information to the service directory
            var client = new RestClient(baseUrl);
            var request = new RestRequest("api/v1/agent/register", Method.POST);
            var body = new ServiceRegistrationRequest();
            body.Kind = "Service";
            body.ApiVersion = "v1";
            
            // This is the URI that should be used to access the services
            body.Spec.Path = config.Keyword(Keys.SERVICE_DIRECTORY__HOST_URI);

            // Certificate / Key that identifies us as a whitelisted entity
            body.MetaData.Whoami = hostIdentity;

            // These are the UPP scopes that this implementation provides access to
            body.MetaData.Labels.Scopes = config.Keyword(Keys.SERVICE_DIRECTORY__SCOPES);

            // We provide UPP services
            body.MetaData.Labels.Type = "upp";

            // Use the Newtonsoft serialize so that we get the data annotations
            request.JsonSerializer = new RestSharpJsonNetSerializer();

            request.AddJsonBody(body);

            logger.Debug("Attempting to register with service directory at {0}", request.Resource);
            logger.Debug("  POST {0}", request.Resource);
            logger.Debug("     uri: {0}", config.Keyword(Keys.SERVICE_DIRECTORY__HOST_URI));
            logger.Debug("  scopes: {0}", config.Keyword(Keys.SERVICE_DIRECTORY__SCOPES));
            logger.Debug("  whoami: {0}", hostIdentity);

            // Sending the request now....
            lastRequest = DateTime.Now;

            // Get the response and see if we were able to register outselves
            var response = client.Execute(request);
            logger.Debug("Service Directory Response: {0}", response.Content);

            // Parse the response
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            // Success!!
            return true;
        }

        public static Func<NancyContext, Response> GetPipelineHook(string hostIdentity, HostConfigurationSection config)
        {
            var handler = new RegisterWithServiceDirectory(hostIdentity, config);

            // Try to make an initial connection to the Service Registry
            handler.InitialRegistration();

            return ctx =>
            {
                // Try to register
                handler.PipelineCallback();

                // Never terminate the pipeline
                return null;
            };
        }
    }
}
