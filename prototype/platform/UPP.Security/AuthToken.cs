using Newtonsoft.Json;
using System;

namespace UPP.Security
{
    /// <summary>
    /// This class described the ID Token format that is sent to UPP clients. Not to be confused with Access Tokens
    /// </summary>
    public sealed class AuthToken
    {
        // "Standard" JWT Claims: https://tools.ietf.org/html/rfc7519#section-4.1.1        
        [JsonProperty("iss")]
        public string Iss { get; set; }
        [JsonProperty("idp")]
        public string Idp { get; set; }
        [JsonProperty("sub")]
        public string Sub { get; set; }
        [JsonProperty("exp")]
        public DateTime Exp { get; set; }

        // UPP Identity
        [JsonProperty("upp")]
        public string Upp { get; set; }

        // External access tokens
        public string Tokens { get; set; }

        // Claims required by Hauler Info
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
