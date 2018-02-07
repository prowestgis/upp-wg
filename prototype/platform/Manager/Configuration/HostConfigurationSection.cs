using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;

namespace Manager.Configuration
{
    public sealed class ManagerConfigurationSection : HostConfigurationSection
    {
        [ConfigurationProperty("microservices", IsRequired = false)]
        public MicroServiceCollection MicroServices
        {
            get { return base["microservices"] as MicroServiceCollection; }
        }
    }
    
    [ConfigurationCollection(typeof(MicroServiceElement))]
    public sealed class MicroServiceCollection : EnumerableConfigurationElementCollection<MicroServiceElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MicroServiceElement)element).Key;
        }
    }

    public sealed class MicroServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)base["key"]; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("oauth-id", IsRequired = false)]
        public string OAuthId
        {
            get { return (string)base["oauth-id"]; }
        }

        [ConfigurationProperty("uri", IsRequired = true)]
        public string Uri
        {
            get { return (string)base["uri"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
        }

        [ConfigurationProperty("priority", IsRequired = true)]
        public int Priority
        {
            get { return (int)base["priority"]; }
        }

        [ConfigurationProperty("active", IsRequired = false, DefaultValue = true)]
        public bool Active
        {
            get { return (bool)base["active"]; }
        }
    }
}
