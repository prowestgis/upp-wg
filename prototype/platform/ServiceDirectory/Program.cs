using System;
using System.Configuration;
using UPP.Configuration;

namespace ServiceDirectory
{
    /// <summary>
    /// A stand-alone server that allows clients to register themselves as providing various UPP
    /// services.  
    /// 
    /// This is an implementation of the "Service register" microservice pattern: http://microservices.io/patterns/service-registry.html
    /// using Self Registration: http://microservices.io/patterns/self-registration.html
    /// 
    /// There are third-party solutions for this problem as well, e.g. https://www.consul.io/
    /// 
    /// An important question is how do the external services authentication themselves in order to prevent random
    /// servers from injecting themselves into this critical infrastructure.
    /// 
    /// Since all of the valid clients are Local Government, we perform a simple validation in the prototype
    /// 
    ///   1. The connection must come from a *.mn.us domain
    ///   2. The connection must use HTTPS
    ///   3. The service register verifies that the IP address of the request matches the DNS records of the client
    ///   4. The service register verified that the SSL certificate of the host is valid
    ///   
    /// This could also be handled via an IP whitelist
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
