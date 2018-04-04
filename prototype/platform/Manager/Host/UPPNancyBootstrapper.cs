using Manager.Security;
using Manager.Store;
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
using System.Collections.Generic;
using System.Configuration;
using UPP.Common;
using UPP.Configuration;
using UPP.Security;

namespace Manager.Host
{
    public sealed class UPPAuthenticationConfigurationOptions : IConfigurationOptions
    {
        public Uri BasePath { get; set; }
    }

    public sealed class UPPNancyBootstrapper : UPP.Common.NancyBootstrapper
    {
        private Store.Services services;

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Bootstrap our application services
            services = new Store.Services();

            // Get the registerd configuration
            var config = container.Resolve<HostConfigurationSection>();
            var hostUri = new Uri(config.Keyword(Keys.NANCY__HOST_URI) ?? config.Keyword(Keys.NANCY__BASE_URI));
            var authConfig = new UPPAuthenticationConfigurationOptions { BasePath = hostUri };

            // Register our own interface for looking up additional claims for users (override default implementation)
            container.Register<IAdditionalClaimProvider, DatabaseAdditionalClaimProvider>();

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

        /// <summary>
        /// Read in the claims from the database
        /// </summary>
        public sealed class DatabaseAdditionalClaimProvider : IAdditionalClaimProvider
        {
            private readonly Services _services;

            public DatabaseAdditionalClaimProvider(Services services)
            {
                _services = services;
            }

            public IDictionary<string, object> FindClaims(AuthToken token)
            {
                return _services.QueryAdditionalClaimsForIdentity(token.Upp);
            }
        }
    }
}
