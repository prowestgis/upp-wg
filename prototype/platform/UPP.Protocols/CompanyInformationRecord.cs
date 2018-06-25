using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    /// <summary>
    /// Returned from authoritative systems to approve a permit request
    /// </summary>
    public sealed class CompanyInformationRecord
    {
        //// Date of approval
        //[JsonProperty(Required = Required.Always, PropertyName = "timestamp")]
        //public DateTime Timestamp { get; set; }

        //[JsonProperty(Required = Required.Always, PropertyName = "status")]
        //[EnumDataType(typeof(PermitApprovalStatus.Status))]
        //public string Status { get; set; }

        //// Identify the source of the approval
        //[JsonProperty(Required = Required.Always, PropertyName = "authority")]
        //public string Authority { get; set; }

        //// Receipt is an opaque data record returned by the authority.
        //[JsonProperty(Required = Required.Always, PropertyName = "receipt")]
        //public string Receipt { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "type")]
        public string Type { get { return "company"; } }

        [JsonProperty(Required = Required.Always, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "attributes")]
        public Attributes CompanyInfo { get; set; }

    }

    public sealed class Attributes
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
    public sealed class CompanyResponse
    {
        public CompanyResponse()
        {
            this.Data = new List<CompanyInformationRecord>();
        }
        public List<CompanyInformationRecord> Data { get; set; }
    }
}
