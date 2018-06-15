using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    public sealed class PermitRequestResponse
    {
        [JsonProperty(Required = Required.Always, PropertyName = "type")]
        public const string Type = "upp.permit-response";

        [JsonProperty(Required = Required.Always, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "attributes")]
        public PermitApprovalRecord Attributes { get; set; }

        [JsonProperty(Required = Required.DisallowNull)]
        public Dictionary<string,string> Links { get; set; }

        public static JSchema JsonSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(PermitRequestResponse));
            schema.Title = UPPContentType.PERMIT_ISSUER_RESPONSE.DisplayText;
            schema.ContentMediaType = UPPContentType.PERMIT_ISSUER_RESPONSE.Key;
            return schema;
        }
    }
}
