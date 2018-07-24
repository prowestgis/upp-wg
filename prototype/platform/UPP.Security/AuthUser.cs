using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Security
{
    public class AuthUser
    {
        // Metainformation
        public string AuthenticationProvider { get; }
        public bool IsAuthenticated { get; }
        public DateTime UtcExpiration { get; }

        // Core claims
        public string Name { get; }
        public string Email { get; }
        public string Phone { get; }

        // Required by IUserIdentity
        public string UserName { get; }
        public IEnumerable<string> Claims { get { return ExtendedClaims.Keys; } }

        // Better way to check claims
        public IDictionary<string, object> ExtendedClaims { get; }

        public AuthUser()
        {
            IsAuthenticated = false;
            ExtendedClaims = new Dictionary<string, object>();
        }

        public AuthUser(AuthToken token)
            : this()
        {
            AuthenticationProvider = token.Iss;
            IsAuthenticated = true;

            Name = token.Sub;
            UserName = token.Sub;
            Email = token.Email;
            Phone = token.Phone;

            // Copy the claims
            ExtendedClaims.Add("iss", token.Iss);
            ExtendedClaims.Add("idp", token.Idp);
            ExtendedClaims.Add("sub", token.Sub);
            ExtendedClaims.Add("exp", token.Exp);
            ExtendedClaims.Add("upp", token.Upp);
            ExtendedClaims.Add("email", token.Email);
            ExtendedClaims.Add("phone", token.Phone);
            ExtendedClaims.Add("tokens", token.Tokens);

            UtcExpiration = token.Exp;
        }

        public void AddClaim(string claim, object value)
        {
            ExtendedClaims.Add(claim, value);
        }

        public bool IsUPPAdmin { get { return IsAuthenticated && ExtendedClaims.ContainsKey(UPP.Security.Claims.UPP_ADMIN); } }
    }
}
