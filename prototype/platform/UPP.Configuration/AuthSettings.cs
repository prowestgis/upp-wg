using System;
using System.Configuration;
using System.Text;

namespace UPP.Configuration
{
    /// <summary>
    /// Mediate access to secret keys
    /// </summary>
    public class AuthSettings
    {
        private static string secretKey = ConfigurationManager.AppSettings[AppKeys.JWT_SIGNING_KEY];

        public AuthSettings()
        {
            SecretKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
        }

        public string SecretKeyBase64 { get; set; }
        public string CookieName => AppKeys.UPP_COOKIE_NAME;

        public byte[] SecretKey
        {
            get
            {
                return Convert.FromBase64String(SecretKeyBase64);
            }
        }
    }
}
