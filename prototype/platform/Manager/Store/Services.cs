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
    public sealed class Services
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

            // Extract the microservice providers
            var section = ConfigurationManager.GetSection("upp") as HostConfigurationSection;
            foreach (var record in section.MicroServices)
            {
                MicroServiceProviders.Add(new MicroServiceProvider(record));
            }
        }

        public Services()
        {            
        }

        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\SimpleDb.sqlite"; }
        }

        public static DbConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }

        public static void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var command = cnn.CreateCommand();
                command.CommandText = File.ReadAllText(@"Store\Schema.sql");
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Collection of the microservices that profice core functionality
        /// 
        /// Each service has an endpoint, an optional OAuth reference (client_id / secret) and priority. The system
        /// will try service in priority order.  If multiple services have the same priority, any of the services
        /// *may* be used.
        /// </summary>
        private static void InitializeServiceProviders()
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
                        INSERT INTO MicroServiceProviders (provider_id, display_name, uri, service_type, service_priority, active)
                        VALUES (@Name, @DisplayName, @Uri, @Type, @Priority, @Active)
                        ";
                    insert.CommandType = System.Data.CommandType.Text;
                    insert.Parameters.Add(new SQLiteParameter("@Name", provider.Name));
                    insert.Parameters.Add(new SQLiteParameter("@DisplayName", provider.DisplayName));
                    insert.Parameters.Add(new SQLiteParameter("@Uri", provider.Uri));
                    insert.Parameters.Add(new SQLiteParameter("@Type", provider.Type));
                    insert.Parameters.Add(new SQLiteParameter("@Priority", provider.Priority));
                    insert.Parameters.Add(new SQLiteParameter("@Active", provider.Active));

                    logger.Debug("Adding '{0}' microservice provider to database", provider.DisplayName);
                    insert.ExecuteNonQuery();
                }
            }
        }

        private static void InitializeOAuthProviders()
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

        private static IEnumerable<MicroServiceProviderConfig> GetMicroServiceProviders()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<MicroServiceProvider>(@"
                    SELECT
                        provider_id AS Name,
                        display_name AS DisplayName,
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

        /// <summary>
        /// Used to populate test data: https://www.mockaroo.com/
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="tableName"></param>
        private static void ImportTableFromCSV(string csvFile, string tableName)
        {
            if (!File.Exists(csvFile))
            {
                logger.Warn("CSV file '{0}' does not exist", csvFile);
                return;
            }

            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvHelper.CsvReader(reader))
            using (var conn = SimpleDbConnection())
            {
                var records = new List<dynamic>();

                // Assume the CSV header matches the database fields
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    records.Add(csv.GetRecord<dynamic>());
                }

                // Extract the header names
                var tableFields = new List<string>();
                var objectFields = new List<string>();
                foreach (var header in csv.Context.HeaderRecord)
                {
                    tableFields.Add(header);
                    objectFields.Add("@" + header);
                }

                // Insert the records
                var sql = String.Format(@"
                    INSERT INTO {0}({1})
                    VALUES ({2})
                ", tableName, String.Join(", ", tableFields), String.Join(", ", objectFields));

                conn.Execute(sql, records);
            }
        }

        private static IEnumerable<OAuthProviderConfig> GetAuthenticationProviders()
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

        public static void RegisterOAuthProviders()
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

                    default:
                        logger.Warn("Unknown OAuth provider: '{0}'. Skipping.", provider.Name);
                        break;
                }
            }
        }

        public static void Bootstrap(TinyIoCContainer container)
        {            
            if (!File.Exists(DbFile))
            {
                logger.Info("Database file does not exist. Creating database at {0}", DbFile);
                CreateDatabase();
            }
            else
            {
                logger.Info("Database exists at {0}", DbFile);
                logger.Info("Deleting database at {0}", DbFile);
                File.Delete(DbFile);
                CreateDatabase();
            }

            // Initialize the database tables
            InitializeOAuthProviders();
            InitializeServiceProviders();

            // Register the OAuth providers with the Nancy middleware
            RegisterOAuthProviders();

            // Copy in test data to populate the database for mocked services
            ImportTableFromCSV(Path.Combine(Environment.CurrentDirectory, @"MockData\COMPANY_DATA.csv"), "CompanyInformation");
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

        public IEnumerable<dynamic> FindCompanyInfoForUser(string uppId)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<dynamic>(@"
                    SELECT
                        company_name AS CompanyName,
                        email AS Email,
                        contact AS Contact,
                        phone AS Phone,
                        fax AS Fax,
                        cell AS Cell,
                        bill_to AS BillTo,
                        billing_address AS BillingAddress
                    FROM CompanyInformation
                    INNER JOIN UserCompanies
                    ON CompanyInformation.company_id = UserCompanies.company_id
                    WHERE user_id = @UppId
                    ",
                    new { UppId = uppId }
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
