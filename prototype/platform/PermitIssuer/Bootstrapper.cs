using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using NLog;
using Newtonsoft.Json;
using UPP.Configuration;
using UPP.Common;
using System.Configuration;

namespace PermitIssuer
{
    public class Bootstrapper : UPP.Common.NancyBootstrapper
    {
        private static string UPP_IDENTITY = ConfigurationManager.AppSettings[AppKeys.UPP_IDENTITY];

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Bootstrap our application services
            //logger.Debug("Registering database singleton");
            //container.Register(new Database());
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Initialize the database
            //logger.Debug("ApplicationStartup: Initializing the database");
            //container.Resolve<Database>().Initialize();

            // Register a callback so we can periodically try to register ourselves with the Service Directory.  This is
            // am improper hijack of the request pipeline, but *much* easier than setting up a real background monitor task
            // for prototype development.
            var config = container.Resolve<HostConfigurationSection>();
            pipelines.BeforeRequest.AddItemToStartOfPipeline(RegisterWithServiceDirectory.GetPipelineHook(UPP_IDENTITY, config));
        }
    }
}
