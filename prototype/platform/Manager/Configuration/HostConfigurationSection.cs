using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Configuration
{
    public static class AppKeys
    {
        public const string GOOGLE_OAUTH_KEY = "UPP:Security:OAuth:Google:Key";
        public const string GOOGLE_OAUTH_SECRET = "UPP:Security:OAuth:Google:Secret";

        public const string JWT_SIGNING_KEY = "UPP:Security:JWT:Key";
    }

    public static class Keys
    {
        public const string NANCY__BASE_URI = "Nancy:BaseUri";

        public static IDictionary<string, string> DefaultValues = new Dictionary<string, string>
        {
            { NANCY__BASE_URI, "http://localhost:56484" }
        };
    }

    public sealed class HostConfigurationSection : ConfigurationSection
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [ConfigurationProperty("keywords", IsRequired = false)]
        public KeywordsCollection Keywords
        {
            get { return base["keywords"] as KeywordsCollection; }
        }

        public string Keyword(string key)
        {
            return Keywords.Where(x => x.Key.Equals(key)).Select(x => x.Value).FirstOrDefault() ?? Keys.DefaultValues[key];
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
