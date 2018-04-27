using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;
using UPP.Protocols;
using System.IO;
using Newtonsoft.Json;

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

        public WebModule(Database database, HostConfigurationSection config)
        {
            // Need a valid JWT to access
            // this.RequiresAuthentication();

            // Provide a view for configuring the issuer's behavior
            Get["/"] = _ => View["Index", new WebModuleViewModel { Configuration = config, ModuleConfiguration = Configuration }];
            Post["/"] = _ => UpdateConfiguration(config);

            Get["/permits"] = _ => View["Permits", new ActivePermitView(database, Context)];
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
        
        // A permit file contains an application and multiple reviews
        public sealed class PermitApplicationContainer
        {
            public long Id { get; set; }
            public string Status { get; set; }
            public PermitApplicationRecord Permit { get; set; }
        }
        
        public sealed class ActivePermitView
        {
            private Newtonsoft.Json.JsonSerializer Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();

            public ActivePermitView(Database database, NancyContext context)
            {
                Permits =
                    database.FindPermitsForUser(context.CurrentUser)
                    .Select(x => new PermitApplicationContainer
                    {
                        Id = x.Id,
                        Status = x.Status.ToString(),
                        Permit = (PermitApplicationRecord)Serializer.Deserialize(new StringReader(x.Data), typeof(PermitApplicationRecord))
                    })
                    .ToList();
            }

            public List<PermitApplicationContainer> Permits { get; set; }
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