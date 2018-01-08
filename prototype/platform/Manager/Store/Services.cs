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

namespace Manager.Store
{
    /// <summary>
    /// Persistent store of service configuration information
    /// </summary>
    public sealed class Services
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<OAuthProvider> OAuthProviders = new List<OAuthProvider>();

        static Services()
        {
            // Extract any OAuth providers passed into the configuration
            var settings = ConfigurationManager.AppSettings;
            var keys = settings.AllKeys;
            if (keys.Contains(AppKeys.GOOGLE_OAUTH_KEY) && keys.Contains(AppKeys.GOOGLE_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("google", settings[AppKeys.GOOGLE_OAUTH_KEY], settings[AppKeys.GOOGLE_OAUTH_SECRET]));
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

                    insert.CommandText = "INSERT INTO OAuth2Providers (provider_id, client_key, client_secret, active) VALUES (@Name, @Key, @Secret, @Active)";
                    insert.CommandType = System.Data.CommandType.Text;
                    insert.Parameters.Add(new SQLiteParameter("@Name", provider.Name));
                    insert.Parameters.Add(new SQLiteParameter("@Key", provider.Key));
                    insert.Parameters.Add(new SQLiteParameter("@Secret", provider.Secret));
                    insert.Parameters.Add(new SQLiteParameter("@Active", provider.Active));

                    logger.Debug("Adding '{0}' OAuth provider to database", provider.Name);
                    insert.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<OAuthProviderConfig> GetAuthenticationProviders()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<OAuthProviderConfig>(
                    "SELECT provider_id AS Name, client_key AS Key, client_secret AS Secret FROM OAuth2Providers WHERE Active = 1"
                    )
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
            }

            // Initialize the database tables
            InitializeOAuthProviders();

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

        private sealed class OAuthProvider
        {
            public OAuthProvider(string name, string key, string secret)
            {
                Name = name;
                Key = key;
                Secret = secret;
                Active = true;
            }

            public string Name { get; set; }
            public string Key { get; set; }
            public string Secret { get; set; }
            public bool Active { get; set; }
        }

        public sealed class OAuthProviderConfig
        {
            public OAuthProviderConfig(string name, string key, string secret)
            {
                Name = name;
                Key = key;
                Secret = secret;
            }

            public string Name { get; set; }
            public string Key { get; set; }
            public string Secret { get; set; }
        }
    }    
}
