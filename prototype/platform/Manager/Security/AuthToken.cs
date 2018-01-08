using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Security
{
    /// <summary>
    /// This class described the token format that is sent to UPP clients
    /// </summary>
    internal sealed class AuthToken
    {
        // JWT Claims
        public string Sub { get; set; }
        public DateTime Exp { get; set; }
        public string[] Claims { get; set; }

        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
