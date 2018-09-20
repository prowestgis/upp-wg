namespace UPP.Security
{
    /// <summary>
    /// Scopes partition the services that a given client (program) is going to ask for.  For example,
    /// a nightly process may connect and query the UPP system to get a report of all permit issues in the
    /// past 24-hours.  They would need to request a token in the "reporting" scope.
    /// 
    /// When a user logs in or exchanges an external token for a UPP token, the platform need to look up the
    /// identity in a UPP data store and check if that identity is authorized for the scopes it is asking for.
    /// </summary>
    public static class Scopes
    {
        // UPP scopes 
        public const string PERMIT_REQUEST = "permit:request";
        public const string PERMIT_REVIEW = "permit:review";
        public const string PERMIT_RENFORCEMENT = "permit:enforcement";

        public const string SERVICES_WRITE = "services:write";
        public const string SERVICES_READ = "services:read";        
    }
}
