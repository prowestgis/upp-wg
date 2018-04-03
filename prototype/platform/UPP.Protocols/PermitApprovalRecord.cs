using System;
using System.Collections.Generic;
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
        public DateTime Timestamp { get; set; }

        public string Status { get; set; }

        // Identify the source of the approval
        public string Identifier { get; set; }
    }

    public static class PermitApprovalStatus
    {
        public const string APPROVED = "Approved";
        public const string DENIED = "Denied";
        public const string NA = "Not Applicable";

        public static string RANDOM
        {
            get
            {
                Random RNG = new Random();
                var rnd = RNG.Next(1, 4);
                if (rnd == 1) return APPROVED;
                if (rnd == 2) return DENIED;
                return NA;
            }
        }
    }
}
