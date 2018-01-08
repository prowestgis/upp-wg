using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.API
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
        // Full acces to all reporting functions
        public const string REPORTING = "reporting";

        // Limitd access to reports that only return aggregates (summaries) of
        // underlying data
        public const string REPORTING_AGGREGATE = "reporting.aggregate";
    }
}
