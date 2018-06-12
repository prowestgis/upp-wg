using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    public sealed class UPPContentType
    {
        public static readonly UPPContentType SERVICE = new UPPContentType(Domain.SERVICE, "application/vnd.upp.service", "UPP Service");
        public static readonly UPPContentType SERVICE_DESCRIPTION = new UPPContentType(Domain.SERVICE_DESCRIPTION, "application/vnd.upp.service-description", "Service Description");
        public static readonly UPPContentType SERVICE_ACCESS_RECORD = new UPPContentType(Domain.SERVICE_ACCESS_RECORD, "application/vnd.upp.service-access-record", "Service Access Record");
        public static readonly UPPContentType PERMIT_ISSUER_METADATA = new UPPContentType(Domain.PERMIT_ISSUER_METADATA, "application/vnd.upp.permit-issuer.metadata", "Permit Issuer Metadata");
        public static readonly UPPContentType PERMIT_REQUEST = new UPPContentType(Domain.PERMIT_REQUEST, "application/vnd.upp.permit-request", "UPP Permit Request");
        public static readonly UPPContentType PERMIT_ISSUER_RESPONSE = new UPPContentType(Domain.PERMIT_ISSUER_RESPONSE, "application/vnd.upp.permit-issuer.response", "Permit Issuer Response");
        

        public enum Domain
        {
            SERVICE,
            SERVICE_DESCRIPTION,
            SERVICE_ACCESS_RECORD,
            PERMIT_ISSUER_METADATA,
            PERMIT_REQUEST,
            PERMIT_ISSUER_RESPONSE
        };

        private static readonly List<UPPContentType> domain = new List<UPPContentType>
        {
            SERVICE,
            SERVICE_DESCRIPTION,
            SERVICE_ACCESS_RECORD,
            PERMIT_ISSUER_METADATA,
            PERMIT_REQUEST,
            PERMIT_ISSUER_RESPONSE
        };

        public static IEnumerable<UPPContentType> AsEnumerable()
        {
            return domain.AsReadOnly();
        }

        private UPPContentType(Domain value, string key, string displayText)
        {
            Value = value;
            Key = key;
            DisplayText = displayText;
        }

        public Domain Value { get; private set; }
        public string Key { get; private set; }
        public string DisplayText { get; private set; }

        // Allow explicit conversion from a string in order to gain type safety
        public static explicit operator UPPContentType(string value)
        {
            return domain.Single(x => x.Key == value);
        }
    }
}
