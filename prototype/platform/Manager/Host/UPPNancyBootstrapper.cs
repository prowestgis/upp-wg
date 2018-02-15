using Manager.Security;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Conventions;
using Nancy.SimpleAuthentication;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using NLog;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Providers;
using System;
using System.Configuration;
using UPP.Common;
using UPP.Configuration;

namespace Manager.Host
{
    public sealed class UPPAuthenticationConfigurationOptions : IConfigurationOptions
    {
        public Uri BasePath { get; set; }
    }

    public sealed class UPPNancyBootstrapper : UPP.Common.NancyBootstrapper
    {
        private Store.Services services;

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

            // Bootstrap our application services
            services = new Store.Services();

            var config = ConfigurationManager.GetSection("upp") as HostConfigurationSection;
            var hostUri = new Uri(config.Keyword(Keys.NANCY__HOST_URI) ?? config.Keyword(Keys.NANCY__BASE_URI));
            var authConfig = new UPPAuthenticationConfigurationOptions { BasePath = hostUri };

            // Make the generic configuration block easily available
            container.Register(config);

            // Register a configuration for the Simple Authentication
            container.Register<IConfigurationOptions>(authConfig);      

            // Bind the callback handler to our own implementation
            container.Register<IAuthenticationCallbackProvider, UPPAuthenticationCallbackProvider>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            services.Initialize();
        }
    }
}
