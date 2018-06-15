using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.Runtime.CompilerServices;

namespace UPP.Common
{
    public sealed class SelectableResponseProcessor : IResponseProcessor
    {
        private readonly ISerializer jsonSerializer;
        private readonly ISerializer xmlSerializer;

        private enum Format
        {
            JSON,
            XML,
            HTML,
            OTHER
        };

        public SelectableResponseProcessor(IEnumerable<ISerializer> serializers)
        {
            // Grab the configures JSON and XML serializers
            jsonSerializer = serializers.FirstOrDefault(x => x.CanSerialize("application/json"));
            xmlSerializer = serializers.FirstOrDefault(x => x.CanSerialize("application/xml"));
        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get
            {
                return Enumerable.Empty<Tuple<string, MediaRange>>();
            }
        }

        private static Format GetFormat(NancyContext context)
        {
            var format = ((string)context.Request.Query.f ?? "").ToLower();

            switch (format)
            {
                case "json":
                case "pjson":
                    return Format.JSON;

                case "xml":
                    return Format.XML;

                case "html":
                    return Format.HTML;

                default:
                    return Format.OTHER;
            }
        }

        private static bool IsJsonMatch(NancyContext context)
        {
            return GetFormat(context) == Format.JSON;
        }

        private static bool IsXmlMatch(NancyContext context)
        {
            return GetFormat(context) == Format.XML;
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (IsJsonMatch(context))
            {
                return new ProcessorMatch
                {
                    RequestedContentTypeResult = MatchResult.DontCare,
                    ModelResult = MatchResult.ExactMatch
                };
            }

            if (IsXmlMatch(context))
            {
                return new ProcessorMatch
                {
                    RequestedContentTypeResult = MatchResult.DontCare,
                    ModelResult = MatchResult.ExactMatch
                };
            }
            
            return ProcessorMatch.None;
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            var format = GetFormat(context);
            var serializer = (format == Format.XML) ? xmlSerializer : jsonSerializer;
            var mimeType = (format == Format.XML) ? "application/xml" : "application/json";

            return new Response
            {
                Contents = stream =>
                {
                    if (model != null)
                    {
                        serializer.Serialize(mimeType, model, stream);
                    }
                },
                ContentType = mimeType,
                StatusCode = HttpStatusCode.OK
            };            
        }
    }
}
