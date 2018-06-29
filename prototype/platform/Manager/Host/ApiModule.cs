using Nancy;
using Nancy.Security;
using Nancy.Authentication.Stateless;
using Nancy.ModelBinding;
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Manager.Store;
using RestSharp;
using NLog;
using UPP.Security;
using System.IO;
using Nancy.Responses;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.factories;
using static Manager.Host.PermitView;
using Newtonsoft.Json.Linq;
using UPP.Configuration;
using LibGit2Sharp;
using UPP.Common;

namespace Manager.Host
{
    /// <summary>
    /// Top-level module that implements all of the specification's API.  This could evenually be linked
    /// to an auto-generated backend.  The API is secure and requires authentication.
    /// </summary>
    public sealed class ApiModule : NancyModule
    {
        public ApiModule(Services services, HostConfigurationSection config) : base("/api")
        {
            //this.RequiresAuthentication();
            Get["/"] = _ => Response.AsJson(new { Message = "Hello", User = Context.CurrentUser });

            // Create a new permit application
            Post["/permits"] = _ => CreatePermitRepository(services, config, Context.CurrentUser as AuthUser);

            // Return a list of permits available to the current user. This returns a list of Resource Identifier Objects
            Get["/permits"] = _ => FindPermits(services, config, Context.CurrentUser as AuthUser);

            // Retrieve a specific permit
            Get["/permits/{guid}"] = _ => FetchPermit(services, config, Context.CurrentUser as AuthUser, _.guid);

            // Modify a permit
            Post["/permits/{guid}/patch"] = _ => UpdatePermit(services, config, Context.CurrentUser as AuthUser, _.guid);

            // Create the current digital permit package
            Post["/permits/{guid}/package"] = _ => GeneratePermit(services, config, Context.CurrentUser as AuthUser);

            // Show the route data for a permit. Route data can only be updated -- it's not a collection
            Get["/permits/{guid}/route"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Put["/permits/{guid}/route"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };

            // Show the authorities for a permit. Allow new authorities to be dynamically added (as a batch)
            Get["/permits/{guid}/authorities"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Post["/permits/{guid}/authorities"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };

            // Show an authority for a permit. Allow the authority's data to be updated
            Get["/permits/{guid}/authorities/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Put["/permits/{guid}/authorities/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Delete["/permits/{guid}/authorities/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };

            // Get the route approved by this authority --- maybe???
            Get["/permits/{guid}/authorities/{name}/route"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };

            // Show extra data for a permit, such as bridges, weather data, etc.  Simple get/put API.
            Get["/permits/{guid}/extra"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Post["/permits/{guid}/extra"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Get["/permits/{guid}/extra/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Put["/permits/{guid}/extra/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Delete["/permits/{guid}/extra/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };

            // Files can be uploaded and attached to the permit
            Get["/permits/{guid}/attachments"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Post["/permits/{guid}/attachments"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Get["/permits/{guid}/attachments/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Put["/permits/{guid}/attachments/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
            Delete["/permits/{guid}/attachments/{name}"] = _ => new Response { StatusCode = HttpStatusCode.NotImplemented };
        }

        private Response FindPermits(Services services, HostConfigurationSection config, AuthUser user)
        {
            // Configuration parameters
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);

            // Find the bundles
            var permitList = services.FindPermitBundles(user.ExtendedClaims["upp"] as string, repoUrlTemplate);

            // Convert to a JSON API object
            var baseUrl = config.Keyword(Keys.NANCY__HOST_BASE_URI);

            var model = new
            {
                Data = permitList.Select(x => new
                {
                    Type = "permit-application",
                    Id = x.PermitId,
                    Links = new
                    {
                        Self = String.Format("{0}api/permits/{1}", baseUrl, x.PermitId),
                        Origin = x.RepositoryUrl
                    }
                })
            };

            return Response.AsJson(model);
        }

        private JObject UpdatePermitSection(Stream source, JObject permit, string jpath)
        {
            using (var sr = new StreamReader(source))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();

                // read the json from a stream
                var formData = JObject.ReadFrom(reader);

                // Replace the target property with this content
                permit.SelectToken(jpath).Replace(formData);

                return permit;
            }
        }

        private Response UpdatePermit(Services services, HostConfigurationSection config, AuthUser user, string permitIdentifier)
        {
            // Configuration parameters
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);

            // Find the record
            var bundle = services.FetchPermitBundle(user.ExtendedClaims["upp"] as string, repoUrlTemplate, permitIdentifier);

            // Generate the repository path
            var repoPath = Path.Combine(workspace, bundle.RepositoryName);

            // If the repository does not already exists, clone it
            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(bundle.RepositoryUrl, repoPath);
            }

            JObject permit = null;
            string path;
            using (var repo = new Repository(repoPath))
            {
                // Update the checkout
                var author = new Signature(user.UserName, user.Email, DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, new PullOptions());

                // Read the current permit file and return
                path = Path.Combine(repo.Info.WorkingDirectory, "permit.json");
                permit = JObject.Parse(File.ReadAllText(path));
            }

            // What document are we updating?
            var section = Request.Query.section.Value as string ?? String.Empty;
            switch(section.ToLower())
            {
                // Form data goes under the main "data" property
                case "form-data":
                    permit = UpdatePermitSection(Request.Body, permit, "data.attributes.form-data");
                    break;

                // Bridges go under the top-level "relationships" property.
                case "bridges":
                    permit = UpdatePermitSection(Request.Body, permit, "relationships.bridge.data");
                    break;

                // The route is a "data" property.
                case "route":
                    permit = UpdatePermitSection(Request.Body, permit, "data.attributes.route");
                    break;

                // Update all authorities
                case "authorities":
                    permit = UpdatePermitSection(Request.Body, permit, "data.attributes.authorities");
                    break;

                // Update one authority
                case "authority":
                    var authority = Request.Query.authority.Value as string;
                    permit = UpdatePermitSection(Request.Body, permit, String.Format("data.attributes.authorities.{0}", authority));
                    break;

                default:
                    return new Response { StatusCode = HttpStatusCode.BadRequest };
            }

            // Serialize the json content back to the file
            File.WriteAllText(path, JsonConvert.SerializeObject(permit, Formatting.Indented));

            // Commit the changes
            using (var repo = new Repository(repoPath))
            {
                Commands.Stage(repo, path);

                var author = new Signature(user.UserName, user.Email, DateTime.Now);
                var committer = author;

                // Commit the files to the and push to the origin
                var commit = repo.Commit("Update the permit form data", author, committer);

                var options = new PushOptions();
                repo.Network.Push(repo.Branches["master"], options);
            }

            return new Response
            {
                StatusCode = HttpStatusCode.OK
            };
        }

        private Response FetchPermit(Services services, HostConfigurationSection config, AuthUser user, string permitIdentifier)
        {
            // Configuration parameters
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);

            // Find the record
            var bundle = services.FetchPermitBundle(user.ExtendedClaims["upp"] as string, repoUrlTemplate, permitIdentifier);

            // Generate the repository path
            var repoPath = Path.Combine(workspace, bundle.RepositoryName);

            // If the repository does not already exists, clone it
            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(bundle.RepositoryUrl, repoPath);
            }

            using (var repo = new Repository(repoPath))
            {
                // Update the checkout
                var author = new Signature(user.UserName, user.Email, DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, new PullOptions());

                // Read the current permit file and return
                var permit = Path.Combine(repo.Info.WorkingDirectory, "permit.json");
                return Response.AsText(File.ReadAllText(permit), "application/json");
            }
        }

        private Response CreatePermitRepository(Services services, HostConfigurationSection config, AuthUser user)
        {
            // Get the location for storing the bare git repositories
            var folder = config.Keyword(Keys.UPP__PERMIT_ROOT_PATH);
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);

            // Assign a repository (folder) name for this user's request
            var bundle = services.CreatePermitBundle(user.ExtendedClaims["upp"] as string, repoUrlTemplate);
            var path = Path.Combine(folder, bundle.RepositoryName);

            // Initialize a bare repository
            var rootedPath = Repository.Init(path, true);

            // Clone the bare repository into a working space. In a real system, the git server will be tied into the UPP security model
            var repoPath = Path.Combine(workspace, bundle.RepositoryName);

            //var co = new CloneOptions();
            //co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "admin", Password = "admin" };
            Repository.Clone(bundle.RepositoryUrl, repoPath);

            // Load the skeleton permit file
            var permitRecord = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("App_Data/templates/permit.json"));

            // Fill in the required information
            permitRecord.links.origin = bundle.RepositoryUrl;
            permitRecord.data.id = bundle.PermitId;

            // Serialize to an actualy file
            var emptyPermit = JsonConvert.SerializeObject(permitRecord, Formatting.Indented);

            using (var repo = new Repository(repoPath))
            {
                var permit = Path.Combine(repo.Info.WorkingDirectory, "permit.json");
                var attachments = Path.Combine(repo.Info.WorkingDirectory, "attachments");

                File.WriteAllText(permit, emptyPermit);
                Directory.CreateDirectory(attachments);

                Commands.Stage(repo, permit);
                Commands.Stage(repo, attachments);

                var author = new Signature(user.UserName, user.Email, DateTime.Now);
                var committer = author;

                // Commit the files to the and push to the origin
                var commit = repo.Commit("Create new permit application", author, committer);

                var options = new PushOptions();
                repo.Network.Push(repo.Branches["master"], options);
            }

            // Return a JSON API object
            var baseUrl = config.Keyword(Keys.NANCY__HOST_BASE_URI);

            var model = new
            {
                Data = new
                {
                    Type = "permit-application",
                    Id = bundle.PermitId,
                    Links = new
                    {
                        Self = String.Format("{0}api/permits/{1}", baseUrl, bundle.PermitId),
                        Origin = bundle.RepositoryUrl
                    }
                }
            };

            return Response.AsJson(model);
        }

        private Response GeneratePermit(Services services, HostConfigurationSection config, AuthUser user)
        {
            var frm = Request.Form;
            // Raw response
            var response = new Response();
            var record = this.Bind<GeneratePermitModel>();
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
                    document.AddSubject("Permit issued for Over-Size, Over-Weight (OSOW) loads");
                    document.AddTitle("OSOW Permit");

                    // Put a QR code in the corner for for law enforcement to quickly pull up a permit. Need perma-link
                    // infrastructure
                    var qrCode = new BarcodeQRCode("https://upp.prowestgis.com/law-enforcement/check?123456ABC", 1, 1, null);
                    var qrCodeImage = qrCode.GetImage();
                    qrCodeImage.SetAbsolutePosition(document.PageSize.Width - 36f - 72f, document.PageSize.Height - 36f - 72f);
                    qrCodeImage.ScalePercent(200);
                    document.Add(qrCodeImage);
                    
                    document.Add(record.HaulerInfo());
                    document.Add(record.CompanyInfo());
                    document.Add(record.InsuranceInfo());
                    document.Add(record.VehicleInfo());
                    document.Add(record.TruckInfo());
                    document.Add(record.AxleInfo());
                    document.Add(record.TrailerInfo());
                    document.Add(record.LoadInfo());
                    document.Add(record.MovementInfo());
                    Paragraph permitRequests = new Paragraph("Permits Requested:");
                    List permitRequestList = new List();
                    permitRequestList.IndentationLeft = 10;
                    foreach (var permitResponse in record.Authority)
                    {
                        permitRequestList.Add(GeneratePermitModel.FormattedListItem(permitResponse));
                    }
                    permitRequests.Add(permitRequestList);
                    document.Add(permitRequests);
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
        }

        // Fields defined in UPP committee specificiation
        public string ApplicantName { get; private set; }
        public DateTime ApplicationDate { get; private set; }
        public string ApplicantEmail { get; private set; }
        public string ApplicantPhone { get; private set; }
        public string ApplicantFax { get; private set; }
    }

    public sealed class GeneratePermitModel
    {
        public string haulerinfoName { get; set; }
        public DateTime haulerinfoDate {get; set;}
        public string haulerinfoemail { get; set; }
        public string haulerinfophone { get; set; }
        public string haulerinfofax { get; set; }
        public string companyinfoName { get; set; }
        public string companyinfoAddress { get; set; }
        public string companyinfoEmail { get; set; }
        public string companyinfoContact { get; set; }
        public string companyinfoPhone { get; set; }
        public string companyinfoFax { get; set; }
        public string companyinfoCell { get; set; }
        public string companyinfoBillTo { get; set; }
        public string companyinfoBillingAddress { get; set; }
        public string insuranceinfoProvider { get; set; }
        public string insuranceinfoAgencyAddress { get; set; }
        public string insuranceinfoPolicyNumber { get; set; }
        public string insuranceinfoInsuredAmount { get; set; }
        public string vehicleinfoYear { get; set; }
        public string vehicleinfoMake { get; set; }
        public string vehicleinfoModel { get; set; }
        public string vehicleinfoType { get; set; }
        public string vehicleinfoLicense { get; set; }
        public string vehicleinfoState { get; set; }
        public string vehicleinfoSerialNumber { get; set; }
        public string vehicleinfoUSDOTNumber { get; set; }
        public double vehicleinfoEmptyWeight { get; set; }
        public double vehicleinfoRegisteredWeight { get; set; }
        public double truckinfoGrossWeight { get; set; }
        public double truckinfoEmptyWeight { get; set; }
        public double truckinfoRegisteredWeight { get; set; }
        public double truckinfoRegulationWeight { get; set; }
        public string truckinfoDimensionSummary { get; set; }
        public string truckinfoDimensionDescription { get; set; }
        public double truckinfoHeight { get; set; }
        public double truckinfoWidth { get; set; }
        public double truckinfoLength { get; set; }
        public double truckinfoFrontOverhang { get; set; }
        public double truckinfoRearOverhang { get; set; }
        public double truckinfoLeftOverhang { get; set; }
        public double truckinfoRightOverhang { get; set; }
        public string truckinfoDiagram { get; set; }
        public string axleinfoDescription { get; set; }
        public string axleinfoWeightPerAxle { get; set; }
        public string axleinfoDescriptionSummary { get; set; }
        public int axleinfoAxleCount { get; set; }
        public int axleinfoGroupCount { get; set; }
        public string axleinfoApproxAxleLength { get; set; }
        public string axleinfoAxleLength { get; set; }
        public string axleinfoMaxAxleWidth { get; set; }
        public string axleinfoMaxAxleWeight { get; set; }
        public string axleinfoTotalAxleWeight { get; set; }
        public string axleinfoAxleGroupSummary { get; set; }
        public string axleinfoAxelsPerGroup { get; set; }
        public string axleinfoAxleGroupTireType { get; set; }
        public string axleinfoAxleGroupWidth { get; set; }
        public string axleinfoAxleOperatingWeights { get; set; }
        public string axleinfoAxleGroupWeight { get; set; }
        public string axleinfoAxleGroupMaxWidth { get; set; }
        public string axleinfoAxleGroupTotalWeight { get; set; }
        public string axleinfoAxleGroupDistance { get; set; }
        public string trailerinfoDescription { get; set; }
        public string trailerinfoMake { get; set; }
        public string trailerinfoModel { get; set; }
        public string trailerinfoType { get; set; }
        public string trailerinfoSerialNumber { get; set; }
        public string trailerinfoLicenseNumber { get; set; }
        public string trailerinfoState { get; set; }
        public string trailerinfoEmptyWeight { get; set; }
        public string trailerinfoRegisteredWeight { get; set; }
        public string trailerinfoRegulationWeight { get; set; }
        public string loadinfoOwner { get; set; }
        public string loadinfoOverSize { get; set; }
        public string loadinfoOverWeight { get; set; }
        public string loadinfoDescription { get; set; }
        public string loadinfoSizeOrModel { get; set; }
        public string loadinfoWeight { get; set; }

        public DateTime? movementinfoStartDate{ get; set; }
        public DateTime? movementinfoEndDate{ get; set; }
        public string movementinfoHaulingHours{ get; set; }
        public string movementinfoOrigin { get; set; }
        public string movementinfoDestination { get; set; }
        public string movementinfoRouteDescription { get; set; }
        public string movementinfoRouteCountyNumbers { get; set; }
        public double movementinfoMileOfCountyRoad { get; set; }
        public double movementinfoRouteLength { get; set; }
        public string movementinfoStateHighwayPermitNumber { get; set; }
        public DateTime? movementinfoStateHiughwayPermitIssued { get; set; }
        public bool? movementinfoNeedPilotCar { get; set; }
        public bool? movementinfoDestinationWithinCityLimits { get; set; }
        public bool? movementinfoDestinationWithinApplyingCounty { get; set; }

        public string[] Authority { get; set; }

        public string[] Bridges { get; set; }

        public Paragraph HaulerInfo()
        {
            Paragraph hauler = new Paragraph("Hauler Information");
            hauler.Add(Environment.NewLine);
            List list = new List();
            list.SetListSymbol("");
            list.IndentationLeft = 10;
            
            list.Add(FormattedListItem(String.Format("Name: {0}", haulerinfoName)));
            list.Add(FormattedListItem(String.Format("Email: {0}", haulerinfoemail)));
            list.Add(FormattedListItem(String.Format("Phone #: {0}", haulerinfophone)));
            list.Add(FormattedListItem(String.Format("Fax: {0}", haulerinfofax)));
            hauler.Add(list);
            return hauler;
        }
        public Paragraph CompanyInfo()
        {
            Paragraph company = new Paragraph("Company Information");
            company.Add(Environment.NewLine);
            List list = new List();
            list.SetListSymbol("");
            list.IndentationLeft = 10;
            list.Add(FormattedListItem(String.Format("Name: {0}", companyinfoName)));
            list.Add(FormattedListItem(String.Format("Address: {0}", companyinfoAddress)));
            list.Add(FormattedListItem(String.Format("Email: {0}", companyinfoEmail)));
            list.Add(FormattedListItem(String.Format("Contact: {0}", companyinfoContact)));
            list.Add(FormattedListItem(String.Format("Phone #: {0}", companyinfoPhone)));
            list.Add(FormattedListItem(String.Format("Fax #: {0}", companyinfoFax)));
            //list.Add(FormattedListItem(String.Format("Cell Phone #: {0}", companyinfoCell)));
            //list.Add(FormattedListItem(String.Format("Bill To: {0}", companyinfoBillTo)));
            //list.Add(FormattedListItem(String.Format("Billing Address: {0}", companyinfoBillingAddress)));

            company.Add(list);
            return company;
        }
        public Paragraph InsuranceInfo()
        {
            Paragraph insurance = new Paragraph("Insurance Information");
            insurance.Add(Environment.NewLine);
            List list = new List();
            list.SetListSymbol("");
            list.IndentationLeft = 10;
            list.Add(FormattedListItem(String.Format("Provider: {0}", insuranceinfoProvider)));
            list.Add(FormattedListItem(String.Format("Agency Address: {0}", insuranceinfoAgencyAddress)));
            list.Add(FormattedListItem(String.Format("Policy #: {0}", insuranceinfoPolicyNumber)));
            //list.Add(FormattedListItem(String.Format("Insured Amount: {0}", insuranceinfoInsuredAmount)));

            insurance.Add(list);
            return insurance;
        }
        public Paragraph VehicleInfo()
        {
            Paragraph vehicle = new Paragraph("Vehicle Information");
            vehicle.Add(Environment.NewLine);
            List list = new List();
            list.SetListSymbol("");
            list.IndentationLeft = 10;
            //list.Add(FormattedListItem(String.Format("Year: {0}", vehicleinfoYear)));
            list.Add(FormattedListItem(String.Format("Make: {0}", insuranceinfoAgencyAddress)));
            //list.Add(FormattedListItem(String.Format("Model: {0}", insuranceinfoPolicyNumber)));
            list.Add(FormattedListItem(String.Format("Type: {0}", vehicleinfoType)));
            list.Add(FormattedListItem(String.Format("License #: {0}", vehicleinfoLicense)));
            list.Add(FormattedListItem(String.Format("State: {0}", vehicleinfoState)));
            list.Add(FormattedListItem(String.Format("Truck Serial #: {0}", vehicleinfoSerialNumber)));
            list.Add(FormattedListItem(String.Format("USDOT #: {0}", vehicleinfoUSDOTNumber)));
            list.Add(FormattedListItem(String.Format("Empty Weight: {0}", vehicleinfoEmptyWeight)));
            //list.Add(FormattedListItem(String.Format("Registered Weight: {0}", vehicleinfoRegisteredWeight)));

            vehicle.Add(list);
            return vehicle;
        }

        public Paragraph TruckInfo()
        {
            Paragraph truck = new Paragraph("Truck Information");
            truck.Add(Environment.NewLine);

            List list = new List();
            list.SetListSymbol(string.Empty);
            list.IndentationLeft = 10;
            list.Add(FormattedListItem(String.Format("Gross Weight: {0}", truckinfoGrossWeight)));
            //list.Add(FormattedListItem(String.Format("EmptyWeight: {0}", truckinfoEmptyWeight)));
            //list.Add(FormattedListItem(String.Format("Registered Weight: {0}", truckinfoRegisteredWeight)));
            //list.Add(FormattedListItem(String.Format("Regulation Weight: {0}", truckinfoRegulationWeight)));
            list.Add(FormattedListItem(String.Format("Dimension Summary: {0}", truckinfoDimensionSummary)));
            list.Add(FormattedListItem(String.Format("Dimension Description: {0}", truckinfoDimensionDescription)));
            list.Add(FormattedListItem(String.Format("Height: {0}", truckinfoHeight)));
            list.Add(FormattedListItem(String.Format("Width: {0}", truckinfoWidth)));
            list.Add(FormattedListItem(String.Format("Length: {0}", truckinfoLength)));
            list.Add(FormattedListItem("Overhang"));
            List overhang = new List();
            overhang.SetListSymbol("");
            overhang.IndentationLeft = 10;

            overhang.Add(FormattedListItem(String.Format("Front: {0}", truckinfoFrontOverhang)));
            overhang.Add(FormattedListItem(String.Format("Rear: {0}", truckinfoRearOverhang)));
            overhang.Add(FormattedListItem(String.Format("Left: {0}", truckinfoLeftOverhang)));
            overhang.Add(FormattedListItem(String.Format("Right: {0}", truckinfoRightOverhang)));
            list.Add(overhang);

            list.Add(FormattedListItem(String.Format("Diagram: {0}", truckinfoDiagram)));
            truck.Add(list);
            return truck;
        }

        public Paragraph AxleInfo()
        {
            Paragraph para = new Paragraph("Axle Information");
            para.Add(Environment.NewLine);

            List list = new List();
            list.SetListSymbol(string.Empty);
            list.IndentationLeft = 10;
            list.Add(FormattedListItem("Description", axleinfoDescription));

            list.Add(FormattedListItem("Weight Per Axle", axleinfoWeightPerAxle));
            //list.Add(FormattedListItem("Description Summary", axleinfoDescriptionSummary));
            list.Add(FormattedListItem("Axle Count", axleinfoAxleCount.ToString()));
            //list.Add(FormattedListItem("Group Count", axleinfoGroupCount.ToString()));
            //list.Add(FormattedListItem("Approximate Axle Length (Total)", axleinfoApproxAxleLength));
            list.Add(FormattedListItem("Axle Length (axle spacing)", axleinfoAxleLength));
            list.Add(FormattedListItem("Max Axle Width", axleinfoMaxAxleWidth));
            list.Add(FormattedListItem("Max Axle Weight", axleinfoMaxAxleWeight));
            list.Add(FormattedListItem("Total Axle Weight", axleinfoTotalAxleWeight));
            //list.Add(FormattedListItem("Axle Group Summary", axleinfoAxleGroupSummary));
            //list.Add(FormattedListItem("Axels Per Group", axleinfoAxelsPerGroup));
            list.Add(FormattedListItem("Axle Group Tire Type", axleinfoAxleGroupTireType));
            //list.Add(FormattedListItem("Axle Group Width", axleinfoAxleGroupWidth));
            //list.Add(FormattedListItem("Axle Operating Weights", axleinfoAxleOperatingWeights));
            //list.Add(FormattedListItem("Axle Group Weight", axleinfoAxleGroupWeight));
            //list.Add(FormattedListItem("Axle Group Max Width", axleinfoAxleGroupMaxWidth));
            //list.Add(FormattedListItem("Axle Group Total Weight", axleinfoAxleGroupTotalWeight));
            //list.Add(FormattedListItem("Axle Group Distance", axleinfoAxleGroupDistance));

            para.Add(list);
            return para;
        }

        public Paragraph TrailerInfo()
        {
            Paragraph para = new Paragraph("Trailer Information");
            para.Add(Environment.NewLine);

            List list = new List();
            list.SetListSymbol(string.Empty);
            list.IndentationLeft = 10;
            //list.Add(FormattedListItem("Description", trailerinfoDescription));
            list.Add(FormattedListItem("Make", trailerinfoMake));
            //list.Add(FormattedListItem("Model", trailerinfoModel));
            list.Add(FormattedListItem("Type", trailerinfoType));
            //list.Add(FormattedListItem("Serial Number", trailerinfoSerialNumber));
            list.Add(FormattedListItem("License Number", trailerinfoLicenseNumber));
            list.Add(FormattedListItem("State", trailerinfoState));
            list.Add(FormattedListItem("Empty Weight", trailerinfoEmptyWeight));
            //list.Add(FormattedListItem("Registered Weight", trailerinfoRegisteredWeight));
            //list.Add(FormattedListItem("Regulation Weight", trailerinfoRegulationWeight));
            para.Add(list);
            return para;
        }

        public Paragraph LoadInfo()
        {
            Paragraph para = new Paragraph("Load Information");
            para.Add(Environment.NewLine);

            List list = new List();
            list.SetListSymbol(string.Empty);
            list.IndentationLeft = 10;

            //list.Add(FormattedListItem("Owner", loadinfoOwner));
            //list.Add(FormattedListItem("Is the load over size? (yes/no)", loadinfoOverSize));
            //list.Add(FormattedListItem("Is the load over weight? (yes/no)", loadinfoOverWeight));
            list.Add(FormattedListItem("Description", loadinfoDescription));
            //list.Add(FormattedListItem("Load Size or Model", loadinfoSizeOrModel));
            //list.Add(FormattedListItem("Weight", loadinfoWeight));

            para.Add(list);
            return para;
        }

        public Paragraph MovementInfo()
        {
            Paragraph para = new Paragraph("Movement Information");
            para.Add(Environment.NewLine);

            List list = new List();
            list.SetListSymbol(string.Empty);
            list.IndentationLeft = 10;

            list.Add(FormattedListItem("Start Date", movementinfoStartDate.HasValue ? movementinfoStartDate.Value.ToShortDateString() : string.Empty));
            list.Add(FormattedListItem("End Date", movementinfoEndDate.HasValue ? movementinfoEndDate.Value.ToShortDateString() : string.Empty));
            list.Add(FormattedListItem("Hauling Hours", movementinfoHaulingHours));
            list.Add(FormattedListItem("Route Origin", movementinfoOrigin));
            list.Add(FormattedListItem("Route Destination", movementinfoDestination));
            list.Add(FormattedListItem("Route Description", movementinfoRouteDescription));
            list.Add(FormattedListItem("County Roads", movementinfoRouteCountyNumbers));
            list.Add(FormattedListItem("Miles on County Roads", movementinfoMileOfCountyRoad.ToString()));
            list.Add(FormattedListItem("Total Route Length", movementinfoRouteLength.ToString()));
            list.Add(FormattedListItem("State Highway Permit Number", movementinfoStateHighwayPermitNumber));
            list.Add(FormattedListItem("State Highway Permit Issued Date", movementinfoStateHiughwayPermitIssued.HasValue ? movementinfoStateHiughwayPermitIssued.Value.ToShortDateString() : string.Empty));
            list.Add(FormattedListItem("Need Pilot Car?", movementinfoNeedPilotCar.HasValue.ToString()));
            list.Add(FormattedListItem("Destination is within City Limits?", movementinfoDestinationWithinCityLimits.HasValue.ToString()));
            list.Add(FormattedListItem("Destination is within the applying County?", movementinfoDestinationWithinApplyingCounty.HasValue.ToString()));

            para.Add(list);

            para.Add(Environment.NewLine);

            para.Add("Bridges:");
            List bridgetList = new List();
            bridgetList.IndentationLeft = 10;
            if (Bridges != null)
            {
                foreach (var bridge in Bridges)
                {
                    JToken token = JObject.Parse(Uri.UnescapeDataString(bridge));
                    string load = ((double)token.SelectToken("LoadRating")).ToString("F2");
                    // "BRIDGE_ID","FEATINT","FACILITY","ALTIRMETH","ALTIRLOAD"
                    string text = string.Format("ID: {0} - {1} / {2} Load Rating: {3} Road Width: {4} Vert Clearance: {5} Record Type: {6}",
                        token.SelectToken("BRKEY"), token.SelectToken("FEATINT"), token.SelectToken("FACILITY"), load, token.SelectToken("RoadWidth"), token.SelectToken("VERT_CLEAR"), token.SelectToken("RECORD_TYPE"));
                    bridgetList.Add(GeneratePermitModel.FormattedListItem(text));
                }
            } else
            {
                bridgetList.Add(GeneratePermitModel.FormattedListItem("None found for the selected route."));
            }

            para.Add(bridgetList);

            return para;
        }

        private ListItem FormattedListItem(string label, string value)
        {
            string text = string.Format("{0}: {1}",label, value);
            ListItem rtn = new ListItem(12, text, FontFactory.GetFont(FontFactory.HELVETICA, 8));
            return rtn;
        }
        public static ListItem FormattedListItem(string text)
        {
            ListItem rtn = new ListItem(12, text, FontFactory.GetFont(FontFactory.HELVETICA, 8));
            return rtn;
        }
    }
    public sealed class PermitResponse
    {
        public string Authority { get; set; }
        public string Status { get; set; }
    }
}
