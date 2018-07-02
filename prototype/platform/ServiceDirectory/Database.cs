using System;
using System.Linq;
using System.Collections.Generic;
using UPP.Configuration;
using UPP.Common;
using Dapper;
using UPP.Protocols;
using UPP.Security;

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
            ImportTableFromCSV(@"App_Data\OAUTH.csv", "OAuth2Providers");
            ImportTableFromCSV(@"App_Data\TOKEN.csv", "TokenProviders");
            ImportTableFromCSV(@"App_Data\MICROSERVICES.csv", "MicroServiceProviders");
        }

        public MicroServiceProviderConfig FindMicroServiceProviderByName(string name)
        {
            return MicroServiceProviders.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<MicroServiceProviderConfig> FindMicroServiceProviderByType(string type)
        {
            return MicroServiceProviders.Where(x => type == null || type == "all" || x.Type == type);
        }

        public OAuthProviderConfig FindOAuthProviderForService(MicroServiceProviderConfig service)
        {
            return OAuthProviders.FirstOrDefault(x => x.Name == service.OAuthId);
        }

        public TokenProviderConfig FindTokenProviderForService(MicroServiceProviderConfig service)
        {
            return TokenProviders.FirstOrDefault(x => x.Name == service.TokenId);
        }


        public object RegisterService(ServiceRegistrationRecord record)
        {
            using (var conn = SimpleDbConnection())
            {
                // Get a reference to the metadata labels
                var labels = record.Metadata.Labels;
                var annotations = record.Metadata.Annotations;
              
                // Cast the type to a registered value
                var type = (ServiceRegistrationType)labels.Type;

                // Extract the table values from the record
                var name = record.Metadata.Name;
                var description = annotations.Description;
                var format = labels.Format;
                var displayName = labels.FriendlyName;
                var url = record.Spec.ExternalName + record.Spec.Path;
                var authority = labels.Authority;
                var priority = annotations.Priority;
                var oauth = annotations.OAuthId;
                var token = annotations.TokenId;
                var scopes = labels.Scopes;

                logger.Debug("Registering new service:");
                logger.Debug("  Display     = {0}", displayName);
                logger.Debug("  Description = {0}", description);
                logger.Debug("  Url         = {0}", url);
                logger.Debug("  Type        = {0}", type.Key);
                logger.Debug("  Format      = {0}", format);
                logger.Debug("  Authority   = {0}", authority);
                logger.Debug("  Priority    = {0}", priority);
                logger.Debug("  Scopes      = {0}", scopes);
                logger.Debug("  OAuth       = {0}", oauth);
                logger.Debug("  Token       = {0}", token);

                // Insert the new values
                logger.Debug("Inserting new services");
                conn.Execute(@"
                    INSERT INTO MicroServiceProviders (
                      name,
                      oauth_provider_id,
                      token_provider_id,
                      display_name,
                      uri,
                      type,
                      format,
                      description,
                      priority,
                      authority,
                      active,
                      scopes
                    )
                    VALUES (@Name, @OAuth, @Token, @DisplayName, @Uri, @Type, @Format, @Description, @Priority, @Authority, @Active, @Scopes)
                    ",
                    new
                    {
                        Name = name,
                        OAuth = oauth,
                        Token = token,
                        DisplayName = displayName,
                        Uri = url,
                        Type = type.Key,
                        Format = format,
                        Description = description,
                        Priority = priority,
                        Authority = authority,
                        Active = 1,
                        Scopes = scopes
                    }
                );

                // Return a response that enumerates that services were registers, which ones were updates and what failed
                return new
                {
                    Added = new List<string>(),
                    Updated = new List<string>(),
                    Removed = new List<string>(),
                    Failed = new List<string>()
                };
            }
        }

        private IEnumerable<TokenProviderConfig> TokenProviders
        {
            get
            {
                using (var conn = SimpleDbConnection())
                {
                    return conn.Query<TokenProviderConfig>(@"
                    SELECT
                        provider_id AS Name,
                        display_name AS DisplayName,
                        token_url AS Url,
                        token_username AS Username,
                        token_password AS Password,
                        scopes AS Scopes
                    FROM TokenProviders
                    WHERE Active = 1
                    "
                    );
                }
            }
        }

        private IEnumerable<OAuthProviderConfig> OAuthProviders
        {
            get
            {
                using (var conn = SimpleDbConnection())
                {
                    return conn.Query<OAuthProviderConfig>(@"
                    SELECT
                        provider_id AS Name,
                        display_name AS DisplayName,
                        client_key AS Key,
                        client_secret AS Secret,
                        scopes AS Scopes
                    FROM OAuth2Providers
                    WHERE Active = 1
                    "
                    );
                }
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
                            name AS Name,
                            display_name AS DisplayName,
                            description AS Description,
                            oauth_provider_id as OAuthId,
                            token_provider_id as TokenId,
                            uri AS Uri,
                            type AS Type,
                            priority AS Priority,
                            authority AS Authority,
                            format AS Format,
                            active AS Active,
                            scopes AS Scopes
                        FROM MicroServiceProviders
                        WHERE Active = 1
                        "
                        );
                }
            }
        }
    }

    public sealed class MicroServiceProviderConfig
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string OAuthId { get; set; }
        public string TokenId { get; set; }
        public string Uri { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Authority { get; set; }
        public string Scopes { get; set; }
        public int Priority { get; set; }
    }    
}
