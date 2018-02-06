using System.Collections.Generic;
using UPP.Configuration;
using Dapper;

namespace ServiceDirectory
{
    public class Database : DataStore
    {
        public Database() : base("ServiceDirectory.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\MICROSERVICES.csv", "MicroServiceProviders");
        }

        public IEnumerable<MicroServiceProviderConfig> MicroServiceProviders
        {
            get
            {
                using (var conn = SimpleDbConnection())
                {
                    return conn.Query<MicroServiceProviderConfig>(@"
                        SELECT
                            provider_id AS Name,
                            display_name AS DisplayName,
                            oauth_provider_id as OAuthId,
                            uri AS Uri,
                            service_type AS Type,
                            service_priority AS Priority,
                            active AS Active
                        FROM MicroServiceProviders
                        WHERE Active = 1
                        "
                        );
                }
            }
        }

        public sealed class MicroServiceProviderConfig
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string OAuthId { get; set; }
            public string Uri { get; set; }
            public string Type { get; set; }
            public int Priority { get; set; }
        }
    }
}
