using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    /// <summary>
    /// Data type posted to the Service Directory from external providers in order
    /// to register their services.
    /// 
    /// Services are limited to those defined in the ServiceRegistrationType class.
    /// Each whitelisted source can register multiple services, but can only 
    /// register a single service of a given type.
    /// </summary>
    public sealed class ServiceRegistrationRecord
    {
        // Type of service being registered
        public string Type { get; set; }

        // Source of the service. Must be whitelisted.
        public string Whoami { get; set; }

        // URI of the service endpoint
        public string Uri { get; set; }

        // Scopes of a service that are supported. MUST be
        // supplied.  Can pass '*' to support all scopes.
        public string Scopes { get; set; }
    }
}
