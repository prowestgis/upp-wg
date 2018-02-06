namespace UPP.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static partial class Keys
    {
        public const string NANCY__BASE_URI = "Nancy:BaseUri";

        public static IDictionary<string, string> DefaultValues = new Dictionary<string, string>
        {
            { NANCY__BASE_URI, "http://localhost:56484" }
        };
    }

    public class HostConfigurationSection : ConfigurationSection
    {
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
