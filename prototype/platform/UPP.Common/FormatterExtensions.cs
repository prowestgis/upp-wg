using Nancy;
using Nancy.Responses;
using Nancy.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UPP.Common
{
    public interface IResourceObject
    {
        object Id { get; }
        string Type { get; }
    }

    public sealed class AttributeResourceObject : IResourceObject
    {
        public object Id { get; set; }
        public string Type { get; set; }
        public object Attributes { get; set; }
    }

    public static class FormatterExtensions
    {
        private static ISerializer jsonSerializer;

        public static Response AsJsonAPI<TModel>(this IResponseFormatter formatter, IEnumerable<TModel> model, HttpStatusCode statusCode = HttpStatusCode.OK) where TModel : IResourceObject
        {
            return _AsJsonAPI(formatter, model, statusCode);
        }

        public static Response AsJsonAPI<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK) where TModel : IResourceObject
        {
            return _AsJsonAPI(formatter, model, statusCode);
        }

        private static Response _AsJsonAPI(IResponseFormatter formatter, object model, HttpStatusCode statusCode)
        {
            // Specification contratins
            // 1. Clients MUST send all JSON API data in request documents with the header Content-Type: application/vnd.api+json 

            var serializer = jsonSerializer ?? (jsonSerializer = formatter.Serializers.FirstOrDefault(x => x.CanSerialize("application/json")));
            var wrapper = new JsonAPIResponse
            {
                Data = model,
                Meta = new object(),
                JsonApi = new object()
            };

            return new JsonResponse(wrapper, serializer)
            {
                StatusCode = statusCode,
                ContentType = "application/vnd.api+json"
            };
        }
    }

    public class JsonAPIResponse
    {
        public object Data { get; set; }
        public object Meta { get; set; }

        [JsonProperty("jsonapi")]
        public object JsonApi { get; set; }
    }
}
