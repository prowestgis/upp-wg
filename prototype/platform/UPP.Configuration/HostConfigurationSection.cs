namespace UPP.Configuration
{
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static partial class Keys
    {
        public const string SELF__IDENTIFIER = "Self:Identifier";
        public const string NANCY__BASE_URI = "Nancy:BaseUri";
        public const string NANCY__HOST_URI = "Nancy:HostUri";
        public const string NANCY__HOST_BASE_URI = "Nancy:HostBaseUri";
        public const string SERVICE_DIRECTORY__BASE_URI = "ServiceDirectory:BaseUri";
        public const string SERVICE_DIRECTORY__HOST_URI = "ServiceDirectory:HostUri";
        public const string SERVICE_DIRECTORY__SCOPES = "ServiceDirectory:Scopes";
        public const string UPP__ADMINISTRATORS = "UPP:Administrators";

        public const string DATABASE__FILE = "Database:File";
        public const string DATABASE__SCHEMA = "Database:Schema";
        public const string DATABASE__DELETE_ON_STARTUP = "Database:DeleteOnStartup";
        public const string DATABASE__CREATE_ON_STARTUP = "Database:CreateOnStartup";

        public static IDictionary<string, string> DefaultValues = new Dictionary<string, string>
        {
            { NANCY__BASE_URI, "http://localhost:56484" },
            { NANCY__HOST_URI, null },
            { NANCY__HOST_BASE_URI, "/" },
            { SERVICE_DIRECTORY__BASE_URI, null },
            { SERVICE_DIRECTORY__HOST_URI, null },
            { SERVICE_DIRECTORY__SCOPES, "" },
            { SELF__IDENTIFIER, null },
            { UPP__ADMINISTRATORS, "" },
            { DATABASE__DELETE_ON_STARTUP, "true" },
            { DATABASE__CREATE_ON_STARTUP, "true" },
            { DATABASE__FILE, null },
            { DATABASE__SCHEMA, null }
        };

        // Get a command line argument.
        public static string CommandLineValue(string key)
        {
            // Format is a simple key=value
            var args = Environment.GetCommandLineArgs();
            return args
                .Where(a => a.StartsWith(key))
                .Select(a => a.Split('=')[1].Trim())
                .FirstOrDefault();
        }
    }

    public class HostConfigurationSection : ConfigurationSection
    {
        private static IDictionary<string, Func<string>> UserDefaultValues = new Dictionary<string, Func<string>>();
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public void Validate()
        {
            foreach (var keyword in Keywords)
            {
                if (!Keys.DefaultValues.ContainsKey(keyword.Key))
                {
                    logger.Warn("Configuration keyword '{0}' has no internal default value", keyword.Key);
                }
            }
        }

        public void SetDefaultValue(string key, Func<string> defaultValue)
        {
            UserDefaultValues[key] = defaultValue;
        }

        [ConfigurationProperty("keywords", IsRequired = false)]
        public KeywordsCollection Keywords
        {
            get { return base["keywords"] as KeywordsCollection; }
        }

        // When we look for a keyword value, we first check the configuration section and then look at the
        // command line for any overrides.  If neither are set, then we take the built-in default value.
        public string Keyword(string key)
        {
            return Keyword(key, () => null);
        }

        public string Keyword(string key, Func<string> defaultValue)
        {
            return
                Keys.CommandLineValue(key) ??
                Keywords.Where(x => x.Key.Equals(key)).Select(x => x.Value).FirstOrDefault() ??
                defaultValue() ??
                (UserDefaultValues.ContainsKey(key) ? UserDefaultValues[key]() : Keys.DefaultValues[key])
                ;
        }

        public T Keyword<T>(string key)
        {
            var strValue = Keywords.Where(x => x.Key.Equals(key)).Select(x => x.Value).FirstOrDefault() ?? Keys.DefaultValues[key];
            return (T)Convert.ChangeType(strValue, typeof(T));
        }
    }

    [ConfigurationCollection(typeof(KeywordElement))]
    public sealed class KeywordsCollection : EnumerableConfigurationElementCollection<KeywordElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeywordElement)element).Key;
        }
    }

    public sealed class KeywordElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)base["key"]; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
        }
    }    
}
