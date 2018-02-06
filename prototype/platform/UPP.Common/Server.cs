using Nancy.Hosting.Self;
using NLog;
using System;
using UPP.Configuration;

namespace UPP.Common
{
    /// <summary>
    /// Basic Nancy Server that knows about UPP configuration options
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

        public Server(HostConfigurationSection config)
        {
            // Get the host URI to bind Nancy to
            var hostUri = config.Keyword(Keys.NANCY__BASE_URI);

            logger.Debug("Creating NancyHost");
            host = new NancyHost(hostConfig, new Uri(hostUri));
        }

        public void Start()
        {
            logger.Debug("Starting services");
            host.Start();
        }

        public void Stop()
        {
            logger.Debug("Stopping services");
            host.Stop();
        }
    }
}