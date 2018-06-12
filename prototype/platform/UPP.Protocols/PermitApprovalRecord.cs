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
    public sealed class PermitApprovalRecord
    {
        // Date of approval
        [JsonProperty(Required = Required.Always, PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        [EnumDataType(typeof(PermitApprovalStatus.Status))]
        public string Status { get; set; }

        // Identify the source of the approval
        [JsonProperty(Required = Required.Always, PropertyName = "authority")]
        public string Authority { get; set; }

        // Receipt is an opaque data record returned by the authority.
        [JsonProperty(Required = Required.Always, PropertyName = "receipt")]
        public string Receipt { get; set; }
    }

    public static class PermitApprovalStatus
    {
        public const string APPROVED = "approved";
        public const string DENIED = "denied";
        public const string NO_AUTHORITY = "no_authority";
        public const string UNDER_REVIEW = "under_review";

        public enum Status { approved, denied, no_authority, under_review }

        public static string RANDOM
        {
            get
            {
                Random RNG = new Random();
                var rnd = RNG.Next(1, 4);
                if (rnd == 1) return APPROVED;
                if (rnd == 2) return DENIED;
                if (rnd == 3) return UNDER_REVIEW;
                return NO_AUTHORITY;
            }
        }
    }
}
