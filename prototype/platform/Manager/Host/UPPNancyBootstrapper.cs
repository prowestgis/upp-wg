using Manager.Security;
using Manager.Serialization;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Conventions;
using Nancy.SimpleAuthentication;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using NLog;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Providers;

namespace Manager.Host
{
    public sealed class UPPNancyBootstrapper : DefaultNancyBootstrapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            // Look for views under the 'Assets' folder
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
            container.Register<JsonSerializer, UPPJsonSerializer>();

            // Bootstrap our application services
            Manager.Store.Services.Bootstrap(container);            

            // Bind the callback handler to our own implementation
            container.Register<IAuthenticationCallbackProvider, UPPAuthenticationCallbackProvider>();
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
