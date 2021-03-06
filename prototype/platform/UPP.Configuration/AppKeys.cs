﻿namespace UPP.Configuration
{
    public static class AppKeys
    {
        public const string GOOGLE_OAUTH_KEY = "UPP:Security:OAuth:Google:Key";
        public const string GOOGLE_OAUTH_SECRET = "UPP:Security:OAuth:Google:Secret";

        public const string GITHUB_OAUTH_KEY = "UPP:Security:OAuth:GitHub:Key";
        public const string GITHUB_OAUTH_SECRET = "UPP:Security:OAuth:GitHub:Secret";

        public const string RTVISION_OAUTH_KEY = "UPP:Security:OAuth:RTVision:Key";
        public const string RTVISION_OAUTH_SECRET = "UPP:Security:OAuth:RTVision:Secret";

        public const string MNDOT_OAUTH_KEY = "UPP:Security:OAuth:MNDoT:Key";
        public const string MNDOT_OAUTH_SECRET = "UPP:Security:OAuth:MNDoT:Secret";

        public const string ARCGISONLINE_OAUTH_KEY = "UPP:Security:OAuth:AGOL:Key";
        public const string ARCGISONLINE_OAUTH_SECRET = "UPP:Security:OAuth:AGOL:Secret";

        public const string JWT_SIGNING_KEY = "UPP:Security:JWT:Key";

        public const string UPP_IDENTITY = "UPP:Identity";   // UUID that identifies this system
        public const string UPP_AUTHORITY = "UPP:Authority";  // Assigned string that identifies the UPP authority

        public const string UPP_COOKIE_NAME = "uppToken";
    }
}
