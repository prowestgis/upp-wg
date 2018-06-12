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
    public sealed class ServiceAccessRecord
    {
        public ServiceAccessRecord(MicroServiceProviderConfig service)
            : this(service, null, false)
        {
        }

        public ServiceAccessRecord(MicroServiceProviderConfig service, string token)
            : this(service, token, true)
        {
        }

        private ServiceAccessRecord(MicroServiceProviderConfig service, string token, bool secured)
        {
            Name = service.Name;
            Url = service.Uri;
            Token = token;
            IsSecured = secured;
        }

        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public string Name { get; private set; }

        [Url]
        [JsonProperty(Required = Required.Always, PropertyName = "url")]
        public string Url { get; private set; }

        [JsonProperty(Required = Required.Always, PropertyName = "token")]
        public string Token { get; private set; }

        [JsonProperty(Required = Required.Always, PropertyName = "isSecured")]
        public bool IsSecured { get; private set; }

        public static JSchema JsonSchema() {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ServiceAccessRecord));
            schema.ContentMediaType = UPPContentType.SERVICE_ACCESS_RECORD.Key;
            return schema;

        }
    }

    public sealed class MicroServiceProviderConfig
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string OAuthId { get; set; }
        public string TokenId { get; set; }
        public string Uri { get; set; }
        public string Type { get; set; }
        public string Scopes { get; set; }
        public int Priority { get; set; }
    }
}
