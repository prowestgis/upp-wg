using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using NLog;
using Newtonsoft.Json;

namespace CompanyInformation
{
    public class Bootstrapper : UPP.Common.NancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Bootstrap our application services
            logger.Debug("Registering database singleton");
            container.Register(new Database());
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Initialize the database
            logger.Debug("ApplicationStartup: Initializing the database");
            container.Resolve<Database>().Initialize();
        }
    }
}
