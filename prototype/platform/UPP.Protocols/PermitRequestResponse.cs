using Newtonsoft.Json;
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
    }
}
