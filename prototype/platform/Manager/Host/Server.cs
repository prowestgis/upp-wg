using Manager.Configuration;
using Nancy;
using Nancy.Hosting.Self;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace Manager.Host
{    
    /// <summary>
    /// Main Nancy Server
    /// </summary>
    public class Server
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private HostConfiguration hostConfig = new HostConfiguration
        {
            UrlReservations = new UrlReservations
            {
                CreateAutomatically = true
            }
        };

        private readonly NancyHost host;
        private readonly Manager.Store.Services database;

        public Server(HostConfigurationSection config)
        {
            // Get the host URI to bind Nancy to
            var hostUri = config.Keyword(Keys.NANCY__BASE_URI);

            logger.Debug("Creating NancyHost on {0}", hostUri);
            host = new NancyHost(hostConfig, new Uri(hostUri));

            logger.Debug("Creating data store instance");
            database = new Manager.Store.Services();
        }

        public void Start()
        {
            logger.Debug("Starting services");

            // Backend initialization
            logger.Debug("Initializing database");
            database.Startup();

            // Start up the Web Server
            host.Start();
        }

        public void Stop()
        {
            logger.Debug("Stopping services");
            host.Stop();
        }
    }
}
