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

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            // Look for views under the 'Assets' folder for project that provide web interfaces
            conventions.ViewLocationConventions.Insert(0, (viewName, model, context) =>
            {
                return string.Concat("Assets/Views/", viewName);
            });

            // Map static assets to specific virtual paths
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/images", "Assets/Images"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/fonts", "Assets/Fonts"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/css", "Assets/Style"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/js", "Assets/Scripts"));
        }

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
