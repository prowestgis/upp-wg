using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Manager.Serialization
{
    public class UPPJsonSerializer : JsonSerializer
    {
        public UPPJsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}
