using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [JsonProperty(Required = Required.Always, PropertyName = "type")]
        public string Type { get; set; }

        // Source of the service. Must be whitelisted.
        [JsonProperty(Required = Required.Always, PropertyName = "whoami")]
        public string Whoami { get; set; }

        // URI of the service endpoint
        [Url]
        [JsonProperty(Required = Required.Always, PropertyName = "uri")]
        public string Uri { get; set; }

        // Scopes of a service that are supported. MUST be
        // supplied.  Can pass '*' to support all scopes.
        [JsonProperty(Required = Required.Always, PropertyName = "scopes")]
        public string Scopes { get; set; }
    }

    public sealed class ServiceRegistrationRequest
    {
        public ServiceRegistrationRequest()
        {
            MetaData = new ServiceRegistrationRecord();
        }

        [JsonProperty(Required = Required.Always, PropertyName = "kind")]
        public string Kind { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "metadata")]
        public ServiceRegistrationRecord MetaData { get; set; }

        public static JSchema JsonSchema ()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ServiceRegistrationRequest));
            schema.ContentMediaType = UPPContentType.SERVICE.Key;
            return schema;
        }
    }
}
