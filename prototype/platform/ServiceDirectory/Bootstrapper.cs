using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using NLog;
using Newtonsoft.Json;

namespace ServiceDirectory
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Use Json.Net serializer instead of the built-in one
            container.Register<JsonSerializer, UPP.Configuration.JsonSerializer>();

            // Bootstrap our application services
            logger.Debug("Registering database singleton");
            container.Register(new Database());

            // Bind the callback handler to our own implementation
            // container.Register<IAuthenticationCallbackProvider, UPPAuthenticationCallbackProvider>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Initialize the database
            logger.Debug("ApplicationStartup: Initializing the database");
            container.Resolve<Database>().Initialize();

            //var identityProvider = container.Resolve<IIdentityProvider>();
            //var statelessAuthConfig = new StatelessAuthenticationConfiguration(identityProvider.GetUserIdentity);

            //StatelessAuthentication.Enable(pipelines, statelessAuthConfig);
        }
    }
}
