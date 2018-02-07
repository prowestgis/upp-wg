using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Conventions;
using Nancy.TinyIoc;
using NLog;
using System.Configuration;
using UPP.Configuration;

namespace UPP.Common
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Use Json.Net serializer instead of the built-in one
            container.Register<Newtonsoft.Json.JsonSerializer, UPP.Configuration.JsonSerializer>();

            // Make the generic configuration block easily available
            container.Register(ConfigurationManager.GetSection("upp") as HostConfigurationSection);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var identityProvider = container.Resolve<IIdentityProvider>();
            var statelessAuthConfig = new StatelessAuthenticationConfiguration(identityProvider.GetUserIdentity);

            StatelessAuthentication.Enable(pipelines, statelessAuthConfig);
        }
    }
}
