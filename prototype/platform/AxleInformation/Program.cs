using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using UPP.Configuration;

namespace AxleInformation
{
    /// <summary>
    /// Microservice to return axle information related to a UPP Identity.  The Interoperability
    /// spec REQUIRES that a user's email address be supported as an identifying property.  The 
    /// UPP JWT can contain multiple email claims.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Load the configuration
            var config = ConfigurationManager.GetSection("upp") as HostConfigurationSection;

            // Start the service
            var server = new UPP.Common.Server(config);
            server.Start();

            Console.WriteLine("Press any key to stop the service");
            Console.Read();
            Console.WriteLine();

            server.Stop();
        }
    }
}
