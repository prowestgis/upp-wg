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
using SimpleAuthentication.ExtraProviders;
using UPP.SimpleAuthentication.Providers;
using UPP.Configuration;

namespace Manager.Store
{
    /// <summary>
    /// Persistent store of service configuration information
    /// </summary>
    public sealed class Services : DataStore
    {
        // Loaded from a configuration file
        private static List<OAuthProvider> OAuthProviders = new List<OAuthProvider>();
        
        static Services()
        {
            // Extract any OAuth login providers passed into the configuration
            var settings = ConfigurationManager.AppSettings;
            var keys = settings.AllKeys;
            if (keys.Contains(AppKeys.GOOGLE_OAUTH_KEY) && keys.Contains(AppKeys.GOOGLE_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("google", settings[AppKeys.GOOGLE_OAUTH_KEY], settings[AppKeys.GOOGLE_OAUTH_SECRET]));
            }
            if (keys.Contains(AppKeys.GITHUB_OAUTH_KEY) && keys.Contains(AppKeys.GITHUB_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("github", settings[AppKeys.GITHUB_OAUTH_KEY], settings[AppKeys.GITHUB_OAUTH_SECRET]));
            }
            /*
            if (keys.Contains(AppKeys.RTVISION_OAUTH_KEY) && keys.Contains(AppKeys.RTVISION_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("rtvision", settings[AppKeys.RTVISION_OAUTH_KEY], settings[AppKeys.RTVISION_OAUTH_SECRET]));
            }

            if (keys.Contains(AppKeys.MNDOT_OAUTH_KEY) && keys.Contains(AppKeys.MNDOT_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("mndot", settings[AppKeys.MNDOT_OAUTH_KEY], settings[AppKeys.MNDOT_OAUTH_SECRET]));
            }
            */

            if (keys.Contains(AppKeys.ARCGISONLINE_OAUTH_KEY) && keys.Contains(AppKeys.ARCGISONLINE_OAUTH_SECRET))
            {
                OAuthProviders.Add(new OAuthProvider("arcgisonline", settings[AppKeys.ARCGISONLINE_OAUTH_KEY], settings[AppKeys.ARCGISONLINE_OAUTH_SECRET]));
            }
        }

        public Services()
            : base("SimpleDb.sqlite", @"App_Data\Schema.sql")
        {            
        }

        public void RegisterOAuthProviders()
        {
            // Add in all of our registered authorization providers (Google, RT Vision, etc.)
            var authenticationProviderFactory = new AuthenticationProviderFactory();

            foreach (var provider in OAuthProviders)
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

                    case "github":
                        // Request the full user scope instead of the default which is user:email
                        List<string> scopes = new List<string>();
                        scopes.Add("user");
                        parameters.Scopes = scopes;

                        authenticationProviderFactory.AddProvider(new GitHubProvider(parameters));
                        break;
                    /*
                case "rtvision":
                    authenticationProviderFactory.AddProvider(new RTVisionProvider(parameters));
                    break;

                case "mndot":
                    authenticationProviderFactory.AddProvider(new RTVisionProvider(parameters));
                    break;
                    */
                    case "arcgisonline":
                        authenticationProviderFactory.AddProvider(new ArcGISOnlineProvider(parameters));
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

            // Register the OAuth providers with the Nancy middleware
            RegisterOAuthProviders();
        }

        public void Startup()
        {            
        }

        public IEnumerable<OAuthProvider> AuthenticationProviders
        {
            get
            {
                return OAuthProviders;
            }
        }

        private sealed class UserRecord
        {
            public string UserId { get; set; }
            public string ExtraClaims { get; set; }
        }

        public string AddClaimToIdentity(string guid, string claim)
        {
            using (var conn = SimpleDbConnection())
            {
                // Search for the user, return null if no user found
                var user = conn.Query<UserRecord>(@"
                    SELECT user_id as UserId, extra_claims as ExtraClaims
                    FROM Users
                    WHERE user_id = @User
                    ",
                    new { User = guid }
                    )
                    .FirstOrDefault();

                if (user == null)
                {
                    return null;
                }

                // Unpack the current claims
                var claims = (user.ExtraClaims ?? String.Empty).Split(' ').Select(x => x.Trim()).ToList();

                // Add the new claim
                claims.Add(claim);

                // Pack into a string
                var newClaims = String.Join(" ", claims);
                    
                // Update the record
                conn.Execute(@"
                    UPDATE Users
                    SET extra_claims = @Claims
                    WHERE user_id = @User
                    ", new { User = guid, Claims = newClaims }
                );

                // Return the updated claims
                return newClaims;
            }
        }

        public IDictionary<string, object> QueryAdditionalClaimsForIdentity(string guid)
        {
            using (var conn = SimpleDbConnection())
            {
                var claims = new Dictionary<string, object>();

                // Search for the user, return null if no user found
                var user = conn.Query<UserRecord>(@"
                    SELECT user_id as UserId, extra_claims as ExtraClaims
                    FROM Users
                    WHERE user_id = @User
                    ",
                    new { User = guid }
                    )
                    .FirstOrDefault();

                if (user == null)
                {
                    return claims;
                }

                // Unpack the current claims
                var extra_claims = (user.ExtraClaims ?? String.Empty).Split(' ').Select(x => x.Trim()).ToList();

                foreach (var claim in extra_claims)
                {
                    claims[claim] = true;
                }
                
                // Return the updated claims
                return claims;
            }
        }

        public string RemoveClaimFromIdentity(string guid, string claim)
        {
            using (var conn = SimpleDbConnection())
            {
                // Search for the user, return null if no user found
                var user = conn.Query<UserRecord>(@"
                    SELECT user_id as UserId, extra_claims as ExtraClaims
                    FROM Users
                    WHERE user_id = @User
                    ",
                    new { User = guid }
                    )
                    .FirstOrDefault();

                if (user == null)
                {
                    return null;
                }

                // Unpack the current claims
                var claims = (user.ExtraClaims ?? String.Empty).Split(' ').Select(x => x.Trim()).ToList();

                // Try to remove the claim
                if (!claims.Remove(claim))
                {
                    // If not found, return the claims unchanged
                    return user.ExtraClaims;
                }

                // Pack into a string
                var newClaims = String.Join(" ", claims);

                // Update the record
                conn.Execute(@"
                    UPDATE Users
                    SET extra_claims = @Claims
                    WHERE user_id = @User
                    ", new { User = guid, Claims = newClaims }
                );

                // Return the updated claims
                return newClaims;
            }
        }

        public void AddToIdentityFromExternalAuth(string guid, string provider, string externalId)
        {
            using (var conn = SimpleDbConnection())
            {
                // Create a new External Login tied to this user
                conn.Execute(@"
                    INSERT INTO ExternalLogins (user_id, provider_id, provider_user_id)
                    VALUES (@User, @Provider, @ExternalId)
                    ", new { User = guid, Provider = provider, ExternalId = externalId }
                );
            }
        }

        public string CreateNewIdentityFromExternalAuth(string provider, string externalId)
        {
            using (var conn = SimpleDbConnection())
            {
                // Create a new User
                var guid = Guid.NewGuid().ToString();
                conn.Execute(@"
                    INSERT INTO Users (user_id, extra_claims)
                    VALUES (@User, null)
                    ", new { User = guid }
                    );

                // Create a new External Login tied to this user
                conn.Execute(@"
                    INSERT INTO ExternalLogins (user_id, provider_id, provider_user_id)
                    VALUES (@User, @Provider, @ExternalId)
                    ", new { User = guid, Provider = provider, ExternalId = externalId }
                    );

                return guid;
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

        public sealed class OAuthProvider
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
    }    
}
