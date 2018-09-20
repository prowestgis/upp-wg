namespace UPP.Security
{
    /// <summary>
    /// Define all of the claims used by UPP. A claim is a statement about a user. 
    /// </summary>
    public static class Claims
    {
        public const string ROLES = "roles";
        public const string TOKENS = "tokens";
        public const string UPP = "upp";
        public const string EMAIL = "email";
        public const string PHONE = "phone";
        public const string SCOPES = "scopes";

        /// <summary>
        /// Valid values for the 'roles' claim
        /// </summary>
        public static class Roles
        {
            public const string HAULER = "hauler";
            public const string DISPATCHER = "dispatcher";
            public const string ENFORCEMENT = "enforcement";
            public const string ISSUER = "issuer";
            public const string UPP_ADMIN = "upp_admin";            
        }
    }   
}
