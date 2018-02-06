using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UPP.Configuration
{
    public class JsonSerializer : Newtonsoft.Json.JsonSerializer
    {
        public JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}
