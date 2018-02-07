using Jose;

namespace UPP.Security
{
    public class JoseNewtonsoftMapper : IJsonMapper
    {
        public T Parse<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
