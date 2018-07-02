using Nancy;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // Get the base url of the service directory host; this is where we send the registration info
            var baseUrl = config.Keyword(Keys.SERVICE_DIRECTORY__BASE_URI);

            // Get the parameters of the service we are registering
            var serviceUri = config.Keyword(Keys.SERVICE_DIRECTORY__HOST_URI);
            var serviceLabel = config.Keyword(Keys.SERVICE_DIRECTORY__LABEL);
            var serviceScopes = config.Keyword(Keys.SERVICE_DIRECTORY__SCOPES);
            var serviceName = config.Keyword(Keys.SERVICE_DIRECTORY__NAME);
            var serviceType = (ServiceRegistrationType)config.Keyword(Keys.SERVICE_DIRECTORY__TYPE);
            var servicePriority = config.Keyword<int>(Keys.SERVICE_DIRECTORY__PRIORITY);
            var serviceFormat = config.Keyword(Keys.SERVICE_DIRECTORY__FORMAT);
            var serviceDescription = config.Keyword(Keys.SERVICE_DIRECTORY__DESCRIPTION);
            var serviceOAuth = config.Keyword(Keys.SERVICE_DIRECTORY__OAUTH);
            var serviceToken = config.Keyword(Keys.SERVICE_DIRECTORY__TOKEN);
            var serviceAuthority = config.Keyword(AppKeys.UPP_AUTHORITY);

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

            if (String.IsNullOrEmpty(serviceScopes))
            {
                logger.Warn("No service scopes are set in the configuration file. Keyword '{0}'", Keys.SERVICE_DIRECTORY__SCOPES);
                return false;
            }

            // Split the service URI into a host and a path
            var _serviceUri = new Uri(serviceUri);
            var serviceHost = String.Format("{0}://{1}", _serviceUri.Scheme, _serviceUri.Authority);
            var servicePath = _serviceUri.AbsolutePath;

            // Construct and serialize the JSON document for registering the service
            var body = new ServiceRegistrationRecord
            {
                Kind = "Service",
                ApiVersion = "v1",
                Metadata = new ServiceRegistrationMetadata
                {
                    Name = serviceName,                // Unique DNS-style service name, e.g. esri.routing, mndot.bridges, upp.data, etc.
                    Uid = hostIdentity,                // RFC 4122 UUID assigned to the UPP Systems, e.g. 123e4567-e89b-12d3-a456-426655440000

                    Labels = new ServiceRegistrationLabels
                    {
                        FriendlyName = serviceLabel,   // Human-readable service name, e.g. "Clearwater County UPP Service"
                        Scopes = serviceScopes,        // space-delimited list of scopes supported by this service
                        Authority = serviceAuthority,  // Assigned name of the UPP authority, e.g. "clearwater_co_mn",
                        Type = serviceType.Key,        // What service is offered? geometry, routing, data, permit, etc.
                        Format = serviceFormat         // Everything using this API provides data services
                    },
                    Annotations = new ServiceRegistrationAnnotations
                    {
                        Description = serviceDescription,
                        Priority = servicePriority,    // Priority of this service
                        OAuthId = serviceOAuth,        // If this service uses one of the OAuth backend providers
                        TokenId = serviceToken         // If this service uses one of the token backend providers
                    }
                },
                Spec = new ServiceRegistrationSpec
                {
                  Type = "ExternalName",
                  ExternalName = serviceHost,
                  Path = servicePath
              }
            };

            // Get just the host name for the services directory REST endpoint
            var _servicesDirectoryUri = new Uri(baseUrl);

            // Try to POST our registration information to the service directory
            var client = new RestClient(String.Format("{0}://{1}", _servicesDirectoryUri.Scheme, _servicesDirectoryUri.Authority));
            var request = new RestRequest(String.Format("{0}/{1}", _servicesDirectoryUri.AbsolutePath, "services"), Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(body);

            logger.Debug("Attempting to register with service directory at {0}{1}", client.BaseUrl, request.Resource);
            logger.Debug("POST {0}", request.Resource);

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
