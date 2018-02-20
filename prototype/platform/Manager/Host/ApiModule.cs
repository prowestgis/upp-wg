using Nancy;
using Nancy.Security;
using Nancy.Authentication.Stateless;
using System;
using System.Linq;
using Manager.API;
using Newtonsoft.Json;
using Manager.Store;
using RestSharp;
using NLog;
using UPP.Security;
using System.IO;
using Nancy.Responses;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Manager.Host
{
    /// <summary>
    /// Top-level module that implements all of the specification's API.  This could evenually be linked
    /// to an auto-generated backend.  The API is secure and requires authentication.
    /// </summary>
    public sealed class ApiModule : NancyModule
    {
        public ApiModule(Services services) : base("/api")
        {
            //this.RequiresAuthentication();
            Get["/"] = _ => Response.AsJson(new { Message = "Hello", User = Context.CurrentUser });

            // Generate a PDF permit
            Get["/permit"] = _ => GeneratePermit();
        }

        private Response GeneratePermit()
        {
            // Raw response
            var response = new Response();

            // Create a simple PDF
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            byte[] pdf;

            using (var memory = new MemoryStream())
            {
                using (var writer = PdfWriter.GetInstance(document, memory))
                {
                    document.Open();

                    document.AddAuthor("Unified Permitting Project");
                    document.AddCreator("UPP Reference Platform");
                    document.AddKeywords("UPP Permit MnDOT");
                    document.AddSubject("PErmit issues for Over-Size, Over-Weight (OSOW) loads");
                    document.AddTitle("OSOW Permit");

                    // Put a QR code in the corner for for law enforcement to quickly pull up a permit. Need perma-link
                    // infrastructure
                    var qrCode = new BarcodeQRCode("https://upp.prowestgis.com/law-enforcement/check?123456ABC", 1, 1, null);
                    var qrCodeImage = qrCode.GetImage();
                    qrCodeImage.SetAbsolutePosition(25, 25);
                    qrCodeImage.ScalePercent(200);
                    document.Add(qrCodeImage);

                    document.Add(new Paragraph("This is a permit"));
                    document.Close();

                    pdf = memory.ToArray();
                }
            }

            var filename = "permit.pdf";
            response.ContentType = MimeTypes.GetMimeType(filename);
            response.Contents = s =>
            {
                using (var memory = new MemoryStream(pdf))
                {
                    memory.CopyTo(s);
                }
            };

            return response;
        }
    }

    /// <summary>
    /// Implement functionality associated with the Hauler Info scope.  
    /// </summary>
    public sealed class HaulerApiInfo : NancyModule
    {
        public HaulerApiInfo(Services services) : base("/api/hauler")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { Claims.HAULER });

            Get["/"] = _ => Response.AsJson(new HaulerInfoView(Context, services));
        }
    }

    public sealed class HaulerInfoView
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HaulerInfoView(NancyContext context, Services services)
        {
            // Need some of our specific methods
            var user = context.CurrentUser as AuthUser;

            if (user == null)
            {
                throw new ArgumentNullException();
            }

            ApplicantName = user.Name;
            ApplicationDate = DateTime.Now;
            ApplicantEmail = user.Email;
            ApplicantPhone = user.Phone;
            ApplicantFax = null;

            // A Hauler need access to a routing service in order to plan their route -- look at the microservices
            // table to find the current route service endpoint
            var routeService = services.MicroServices.Where(x => x.Type == "route").FirstOrDefault();

            // If there is an OAuth ID, look up the OAuth provider
            if (routeService != null)
            {
                RouteUrl = routeService.Uri;
                if (!String.IsNullOrWhiteSpace(routeService.OAuthId))
                {
                    var oauth = services.AuthenticationProviders.Where(x => x.Name == routeService.OAuthId).FirstOrDefault();
                    if (oauth != null)
                    {
                        // Get a token from ArcGIS Online
                        if (oauth.Name == "agol")
                        {
                            var client = new RestClient("https://www.arcgis.com");
                            var request = new RestRequest("sharing/rest/oauth2/token/", Method.POST);
                            request.AddParameter("client_id", oauth.Key);
                            request.AddParameter("client_secret", oauth.Secret);
                            request.AddParameter("grant_type", "client_credentials");

                            var response = client.Execute(request);
                            var payload = JsonConvert.DeserializeObject<AgolOAuthResponse>(response.Content);

                            RouteToken = payload.access_token;
                        }
                        else
                        {
                            logger.Debug("UPP is not configured to acquire tokens from '{0}'", oauth.Name);
                        }
                    }
                    else
                    {
                        logger.Debug("No matching OAuth provider found for '{0}'", routeService.OAuthId);
                    }
                }
                else
                {
                    logger.Debug("route service is unsecured");
                }
            }
            else
            {
                logger.Debug("No route microservice provider found");
            }
        }

        // Fields defined in UPP committee specificiation
        public string ApplicantName { get; private set; }
        public DateTime ApplicationDate { get; private set; }
        public string ApplicantEmail { get; private set; }
        public string ApplicantPhone { get; private set; }
        public string ApplicantFax { get; private set; }

        public string RouteUrl { get; private set; }
        public string RouteToken { get; private set; }

        public class AgolOAuthResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }
    }
}
