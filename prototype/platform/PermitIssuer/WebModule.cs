using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace PermitIssuer
{
    public sealed class WebModule : NancyModule
    {
        internal static ModuleConfiguration Configuration = new ModuleConfiguration();

        public sealed class WebModuleViewModel
        {
            public HostConfigurationSection Configuration { get; set; }
            public ModuleConfiguration ModuleConfiguration { get; set; }
        }

        public WebModule(HostConfigurationSection config)
        {
            // Need a valid JWT to access
            // this.RequiresAuthentication();

            // Provide a view for configuring the issuer's behavior
            Get["/"] = _ => View["Index", new WebModuleViewModel { Configuration = config, ModuleConfiguration = Configuration }];
            Post["/"] = _ => UpdateConfiguration(config);
        }

        private Response UpdateConfiguration(HostConfigurationSection config)
        {
            // Bind the request to the configuration update record
            var record = this.Bind<ConfigurationUpdateRecord>();

            // Update the module configuration
            Configuration.Behavior = record.Behavior;

            // Redirect to the main page
            return new Nancy.Responses.RedirectResponse("/");
        }

        public sealed class ModuleConfiguration
        {
            public string Behavior { get; set; } = "never";
        }

        public sealed class ConfigurationUpdateRecord
        {
            public string Behavior { get; set; }
        }

    }
}