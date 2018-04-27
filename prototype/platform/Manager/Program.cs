using Manager.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace Manager
{
    static class Program
    {
        private static void StartServer(HostConfigurationSection config)
        {
            var server = new Server(config);

            server.Start();

            Console.WriteLine("Press any key to stop the services");
            Console.Read();
            Console.WriteLine();

            server.Stop();
        }

        static void Main(string[] args)
        {
            // Prevent .Net 4.6 from trying to use SSLv3
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Hack for testing.
            if (args.Length > 0)
            {
                var publicKey = UPP.Security.Https.GetPublicKeyFromUrl(args[0]);
                Console.WriteLine(String.Concat(Array.ConvertAll(publicKey, x => x.ToString("X2"))));
                return;
            }

            // Load the configuration
            var config = ConfigurationManager.GetSection("upp") as HostConfigurationSection;
            StartServer(config);
        }
    }
}
