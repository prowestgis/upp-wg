using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Security
{
    /// <summary>
    /// This class described the ID Token format that is sent to UPP clients. Not to be confused with Access Tokens
    /// </summary>
    public sealed class AuthToken
    {
        // "Standard" JWT Claims: https://tools.ietf.org/html/rfc7519#section-4.1.1
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Exp { get; set; }

        // UPP Identity
        public string Upp { get; set; }

        // Claims required by Hauler Info
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
