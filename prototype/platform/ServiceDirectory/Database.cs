using System;
using System.Linq;
using System.Collections.Generic;
using UPP.Configuration;
using UPP.Common;
using Dapper;
using UPP.Protocols;

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

        public object RegisterService(ServiceRegistrationRecord record)
        {
            using (var conn = SimpleDbConnection())
            {
                // Cast the type to a registered value
                var type = (ServiceRegistrationType)record.Type;

                // Create a unique ID from the type and whoami
                var identifier = String.Format("{0}_{1}", record.Whoami, type.Key).ToLower();

                // Build up a list of scoped identifiers and check to see if any of them are currently registered
                var scopedIdentifiers = record.Scopes.FromCSV().Select(scope => identifier + "_" + scope).ToList();

                // Find any existing records
                logger.Debug("Checking for existing services");
                var updateIdentifiers = conn.Query<string>(@"
                    SELECT provider_id
                    FROM MicroServiceProviders
                    WHERE provider_id IN @Identifiers
                    ",
                    new { Identifiers = scopedIdentifiers })
                    .ToList();

                // Get the list of new services
                var insertIdentifiers = scopedIdentifiers.Where(x => !updateIdentifiers.Contains(x)).ToList();

                // Insert the new values
                logger.Debug("Inserting new services");
                conn.Execute(@"
                    INSERT INTO MicroServiceProviders (provider_id, display_name, uri, service_type, service_priority, active)
                    VALUES (@Id, @Name, @Uri, @Type, @Priority, @Active)
                    ",
                    insertIdentifiers.Select(id => new
                    {
                        Id = id,
                        Name = type.DisplayText,
                        Uri = record.Uri,
                        Type = type.Key,
                        Priority = 1,
                        Active = 1
                    })
                );

                // Update the old values
                logger.Debug("Updating existing services");
                conn.Execute(@"
                    UPDATE MicroServiceProviders
                    SET
                        display_name     = @Name,
                        uri              = @Uri,
                        service_type     = @Type,
                        service_priority = @Priority,
                        active           = @Active
                    WHERE
                        provider_id = @Id
                    ",
                    updateIdentifiers.Select(id => new
                    {
                        Id = id,
                        Name = type.DisplayText,
                        Uri = record.Uri,
                        Type = type.Key,
                        Priority = 1,
                        Active = 1
                    })
                );

                // Return a response that enumerates that services were registers, which ones were updates and what failed
                return new
                {
                    Added = insertIdentifiers,
                    Updated = updateIdentifiers,
                    Removed = new List<string>(),
                    Failed = new List<string>()
                };
            }
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
