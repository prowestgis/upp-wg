using Manager.Configuration;
using Dapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.TinyIoc;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Providers;
using UPP.SimpleAuthentication.Providers;
using UPP.Configuration;

namespace Manager.Store
{
    public static class MicroServiceTypes
    {
        // Provide ESRI-compatible routing services via the 'solve' Network Analyst REST API
        public const string ROUTING = "route";

        // Provides access to state county boundaries in order to identify route segments that
        // intersect each jusrisdiction via an ESRI-compatible map service.
        public const string COUNTY_BOUNDARIES = "county.boundaries";

        // Geometry services for performing general functions
        public const string GEOMETRY = "geometry";
    }

    /// <summary>
    /// Persistent store of service configuration information
    /// </summary>
    public sealed class Services : DataStore
    {
        private static List<OAuthProvider> OAuthProviders = new List<OAuthProvider>();
        private static List<MicroServiceProvider> MicroServiceProviders = new List<MicroServiceProvider>();

        static Services()
        {
            // Extract any OAuth providers passed into the configuration
            var settings = ConfigurationManager.AppSettings;
            var keys = settings.AllKeys;
            if (keys.Contains(AppKeys.GOOGLE_OAUTH_KEY) && keys.Contains(AppKeys.GOOGLE_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("google", settings[AppKeys.GOOGLE_OAUTH_KEY], settings[AppKeys.GOOGLE_OAUTH_SECRET]));
            }

            if (keys.Contains(AppKeys.RTVISION_OAUTH_KEY) && keys.Contains(AppKeys.RTVISION_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("rtvision", settings[AppKeys.RTVISION_OAUTH_KEY], settings[AppKeys.RTVISION_OAUTH_SECRET]));
            }

            if (keys.Contains(AppKeys.MNDOT_OAUTH_KEY) && keys.Contains(AppKeys.MNDOT_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("mndot", settings[AppKeys.MNDOT_OAUTH_KEY], settings[AppKeys.MNDOT_OAUTH_SECRET]));
            }

            if (keys.Contains(AppKeys.ARCGISONLINE_OAUTH_KEY) && keys.Contains(AppKeys.ARCGISONLINE_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("agol", settings[AppKeys.ARCGISONLINE_OAUTH_KEY], settings[AppKeys.ARCGISONLINE_OAUTH_SECRET]));
            }

            // Extract the microservice providers
            var section = ConfigurationManager.GetSection("upp") as ManagerConfigurationSection;
            foreach (var record in section.MicroServices)
            {
                MicroServiceProviders.Add(new MicroServiceProvider(record));
            }
        }

        public Services()
            : base("SimpleDb.sqlite", @"Store\Schema.sql")
        {            
        }

        /// <summary>
        /// Collection of the microservices that profice core functionality
        /// 
        /// Each service has an endpoint, an optional OAuth reference (client_id / secret) and priority. The system
        /// will try service in priority order.  If multiple services have the same priority, any of the services
        /// *may* be used.
        /// </summary>
        private void InitializeServiceProviders()
        {
            using (var cnn = SimpleDbConnection())
            {
                // Open the connection
                cnn.Open();

                // Clear the OAuth2Providers table                                
                var command = cnn.CreateCommand();
                command.CommandText = "DELETE FROM MicroServiceProviders";
                command.ExecuteNonQuery();

                // Run through the providers list and insert each one
                foreach (var provider in MicroServiceProviders)
                {
                    var insert = cnn.CreateCommand();

                    insert.CommandText = @"
                        INSERT INTO MicroServiceProviders (provider_id, display_name, oauth_provider_id, uri, service_type, service_priority, active)
                        VALUES (@Name, @DisplayName, @OAuthId, @Uri, @Type, @Priority, @Active)
                        ";
                    insert.CommandType = System.Data.CommandType.Text;
                    insert.Parameters.Add(new SQLiteParameter("@Name", provider.Name));
                    insert.Parameters.Add(new SQLiteParameter("@DisplayName", provider.DisplayName));
                    insert.Parameters.Add(new SQLiteParameter("@OAuthId", provider.OAuthId));
                    insert.Parameters.Add(new SQLiteParameter("@Uri", provider.Uri));
                    insert.Parameters.Add(new SQLiteParameter("@Type", provider.Type));
                    insert.Parameters.Add(new SQLiteParameter("@Priority", provider.Priority));
                    insert.Parameters.Add(new SQLiteParameter("@Active", provider.Active));

                    logger.Debug("Adding '{0}' microservice provider to database", provider.DisplayName);
                    insert.ExecuteNonQuery();
                }
            }
        }

        private void InitializeOAuthProviders()
        {
            using (var cnn = SimpleDbConnection())
            {
                // Open the connection
                cnn.Open();

                // Clear the OAuth2Providers table                                
                var command = cnn.CreateCommand();
                command.CommandText = "DELETE FROM OAuth2Providers";
                command.ExecuteNonQuery();

                // Run through the providers list and insert each one
                foreach (var provider in OAuthProviders)
                {
                    var insert = cnn.CreateCommand();

                    insert.CommandText = @"
                        INSERT INTO OAuth2Providers (provider_id, display_name, client_key, client_secret, active)
                        VALUES (@Name, @DisplayName, @Key, @Secret, @Active)
                        ";
                    insert.CommandType = System.Data.CommandType.Text;
                    insert.Parameters.Add(new SQLiteParameter("@Name", provider.Name));
                    insert.Parameters.Add(new SQLiteParameter("@DisplayName", provider.DisplayName));
                    insert.Parameters.Add(new SQLiteParameter("@Key", provider.Key));
                    insert.Parameters.Add(new SQLiteParameter("@Secret", provider.Secret));
                    insert.Parameters.Add(new SQLiteParameter("@Active", provider.Active));

                    logger.Debug("Adding '{0}' OAuth provider to database", provider.Name);
                    insert.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<MicroServiceProviderConfig> GetMicroServiceProviders()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<MicroServiceProvider>(@"
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
                    )
                    .Select(x => new MicroServiceProviderConfig(x))
                    .ToList();
            }
        }
       
        private IEnumerable<OAuthProviderConfig> GetAuthenticationProviders()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<OAuthProvider>(@"
                    SELECT
                        provider_id AS Name,
                        client_key AS Key,
                        client_secret AS Secret
                    FROM OAuth2Providers
                    WHERE Active = 1
                    "
                    )
                    .Select(x => new OAuthProviderConfig(x))
                    .ToList();
            }
        }

        public void RegisterOAuthProviders()
        {
            // Add in all of our registered authorization providers (Google, RT Vision, etc.)
            var authenticationProviderFactory = new AuthenticationProviderFactory();

            foreach (var provider in GetAuthenticationProviders())
            {
                var parameters = new ProviderParams
                {
                    PublicApiKey = provider.Key,
                    SecretApiKey = provider.Secret
                };

                switch (provider.Name.ToLower())
                {
                    case "google":
                        authenticationProviderFactory.AddProvider(new GoogleProvider(parameters));
                        break;

                    case "rtvision":
                        authenticationProviderFactory.AddProvider(new RTVisionProvider(parameters));
                        break;

                    case "mndot":
                        authenticationProviderFactory.AddProvider(new RTVisionProvider(parameters));
                        break;

                    case "agol":
                        authenticationProviderFactory.AddProvider(new RTVisionProvider(parameters));
                        break;

                    default:
                        logger.Warn("Unknown OAuth provider: '{0}'. Skipping.", provider.Name);
                        break;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            // Initialize the database tables
            InitializeOAuthProviders();
            InitializeServiceProviders();

            // Register the OAuth providers with the Nancy middleware
            RegisterOAuthProviders();
        }

        public void Startup()
        {            
        }

        public IEnumerable<OAuthProviderConfig> AuthenticationProviders
        {
            get
            {
                return GetAuthenticationProviders();
            }
        }

        public IEnumerable<MicroServiceProviderConfig> MicroServices
        {
            get
            {
                return GetMicroServiceProviders();
            }
        }

        public string CreateNewIdentityFromExternalAuth(string provider, string externalId)
        {
            using (var conn = SimpleDbConnection())
            {
                // Create a new User
                var guid = Guid.NewGuid().ToString();
                conn.Execute(@"INSERT INTO Users (user_id) VALUES (@User)", new { User = guid });

                // Create a new External Login tied to this user
                conn.Execute(@"
                    INSERT INTO ExternalLogins (user_id, provider_id, provider_user_id)
                    VALUES (@User, @Provider, @ExternalId)
                    ", new { User = guid, Provider = provider, ExternalId = externalId }
                    );

                return guid;
            }
        }

        public void AssignUserToCompanies(string uppId, int[] companyIDs)
        {
            using (var conn = SimpleDbConnection())
            {
                var insertList = companyIDs.Select(id => new { User = uppId, Company = id });
                conn.Execute(@"
                    INSERT INTO UserCompanies (user_id, company_id)
                    VALUES (@User, @Company)
                ", insertList
                );
            }
        }

        

        public string FindExternalUser(string provider, string externalId)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<string>(@"
                    SELECT user_id
                    FROM ExternalLogins
                    WHERE provider_id = @Provider AND provider_user_id = @ProviderUserId
                    ",
                    new { Provider = provider, ProviderUserId = externalId }
                    )
                    .FirstOrDefault();
            }
        }

        internal sealed class MicroServiceProvider
        {
            internal MicroServiceProvider()
            {
            }

            public MicroServiceProvider(MicroServiceElement element)
            {
                Name = element.Key;
                DisplayName = element.Name;
                OAuthId = element.OAuthId;
                Uri = element.Uri;
                Type = element.Type;
                Priority = element.Priority;
                Active = element.Active;
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string OAuthId { get; set; }
            public string Uri { get; set; }
            public string Type { get; set; }
            public int Priority { get; set; }
            public bool Active { get; set; }
        }

        public sealed class MicroServiceProviderConfig
        {
            internal MicroServiceProviderConfig(MicroServiceProvider provider)
            {
                Name = provider.Name;
                DisplayName = provider.DisplayName;
                OAuthId = provider.OAuthId;
                Uri = provider.Uri;
                Type = provider.Type;
                Priority = provider.Priority;
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string OAuthId { get; set; }
            public string Uri { get; set; }
            public string Type { get; set; }
            public int Priority { get; set; }
        }

        internal sealed class OAuthProvider
        {
            internal OAuthProvider()
            {
            }

            public OAuthProvider(string name, string key, string secret, string displayName = null)
            {
                Name = name;
                DisplayName = displayName ?? name;
                Key = key;
                Secret = secret;
                Active = true;
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Key { get; set; }
            public string Secret { get; set; }
            public string Scopes { get; set; }
            public bool Active { get; set; }
        }

        public sealed class OAuthProviderConfig
        {
            internal OAuthProviderConfig(OAuthProvider provider)
            {
                Name = provider.Name;
                DisplayName = provider.DisplayName;
                Key = provider.Key;
                Secret = provider.Secret;
                Scopes = provider.Scopes;
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Key { get; set; }
            public string Secret { get; set; }
            public string Scopes { get; set; }
        }
    }    
}
