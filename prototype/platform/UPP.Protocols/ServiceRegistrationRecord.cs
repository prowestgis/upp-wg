using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    /// <summary>
    /// Data type posted to the Service Directory from external providers in order
    /// to register their services.
    /// 
    /// Services are limited to those defined in the ServiceRegistrationType class.
    /// Each whitelisted source can register multiple services, but can only 
    /// register a single service of a given type.
    /// </summary>
    public sealed class ServiceRegistrationRecord
    {
        public string Kind { get; set; }
        public string ApiVersion { get; set; }
        public ServiceRegistrationMetadata Metadata { get; set; }
        public ServiceRegistrationSpec Spec { get; set; }
    }

    public sealed class ServiceRegistrationMetadata
    {
        public string Name { get; set; }
        public string Uid { get; set; }
        public ServiceRegistrationLabels Labels { get; set; }
        public ServiceRegistrationAnnotations Annotations { get; set; }
    }

    public sealed class ServiceRegistrationSpec
    {
        public string Type { get; set; }
        public string ExternalName { get; set; }
        public string Path { get; set; }
    }

    public sealed class ServiceRegistrationLabels
    {
        public string FriendlyName { get; set; }
        public string Scopes { get; set; }
        public string Authority { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
    }

    public sealed class ServiceRegistrationAnnotations
    {
        public string Description { get; set; }
        public int Priority { get; set; }
        public string OAuthId { get; set; }
        public string TokenId { get; set; }
    }
}