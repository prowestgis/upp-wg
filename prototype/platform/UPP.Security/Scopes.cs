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
        // These scopes are defined by the Standard Committee Results spreadsheet
        public const string INFORMATION = "information";
        public const string INFORMATION_HAULER    = INFORMATION + ".hauler";
        public const string INFORMATION_COMPANY   = INFORMATION + ".company";
        public const string INFORMATION_INSURANCE = INFORMATION + ".insurance";
        public const string INFORMATION_VEHICLE   = INFORMATION + ".vehicle";
        public const string INFORMATION_TRUCK     = INFORMATION + ".truck";
        public const string INFORMATION_AXLE      = INFORMATION + ".axle";
        public const string INFORMATION_TRAILER   = INFORMATION + ".trailer";
        public const string INFORMATION_LOAD      = INFORMATION + ".load";
        public const string INFORMATION_MOVEMENT  = INFORMATION + ".movement";

        public const string PERMIT = "permit";
        public const string PERMIT_GENERAL  = PERMIT + ".general";
        public const string PERMIT_APPROVAL = PERMIT + ".approval";
        public const string PERMIT_PAYMENT  = PERMIT + ".payment";
        public const string PERMIT_MISC     = PERMIT + ".misc";


        // Full acces to all reporting functions
        public const string REPORTING = "reporting";

        // Limitd access to reports that only return aggregates (summaries) of
        // underlying data
        public const string REPORTING_AGGREGATE = REPORTING + ".aggregate";
    }
}
