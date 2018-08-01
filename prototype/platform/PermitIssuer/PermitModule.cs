using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Nancy.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UPP.Configuration;
using UPP.Protocols;
using UPP.Security;
using LibGit2Sharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PermitIssuer
{

    public static class PermitHelpers
    {
        public static string ToJson(this object item)
        {
            using (var writer = new StringWriter())
            {
                var serializer = UPP.Configuration.JsonSerializer.CreateDefault();
                serializer.Serialize(writer, item);
                return writer.ToString();
            }
        }
        public static IPermitApplicationRecord ToPermitApplicationRecord(this NancyContext context)
        {
            var form = context.Request.Form;
            return new PermitApplicationRecord
            {
                Type = form["type"],
                Id = form["id"],
                Links = new LinkInfo
                {
                    Origin = form["links[origin]"],
                    Self = form["links[self]"]
                }
            };

        }
        public static IPermitDataRecord ToPermitDataRecord(this NancyContext context)
        {
            var form = context.Request.Form;

            return new PermitDataRecord
            {

                Hauler = new HaulerInfo
                {
                    Name = form["haulerInfo.name"],
                    Date = form["haulerInfo.date"],
                    Email = form["haulerInfo.email"],
                    Phone = form["haulerInfo.phone"],
                    Fax = form["haulerInfo.fax"]
                },
                Company = new CompanyInfo
                {
                    Name = form["companyInfo.name"],
                    Address = form["companyInfo.address"],
                    Email = form["companyInfo.email"],
                    Contact = form["companyInfo.contact"],
                    Phone = form["companyInfo.phone"],
                    Fax = form["companyInfo.fax"]
                },
                Insurance = new InsuranceInfo
                {
                    Provider = form["insuranceInfo.provider"],
                    AgencyAddress = form["insuranceInfo.agencyAddress"],
                    PolicyNumber = form["insuranceInfo.policyNumber"]
                },
                Vehicle = new VehicleInfo
                {
                    Make = form["vehicleInfo.make"],
                    Type = form["vehicleInfo.type"],
                    License = form["vehicleInfo.license"],
                    State = form["vehicleInfo.state"],
                    SerialNumber = form["vehicleInfo.serialNumber"],
                    USDOTNumber = form["vehicleInfo.USDOTNumber"],
                    EmptyWeight = form["vehicleInfo.emptyWeight"],
                },
                Truck = new TruckInfo
                {
                    GrossWeight = form["truckInfo.grossWeight"],
                    DimensionSummary = form["truckInfo.dimensionSummary"],
                    DimensionDescription = form["truckInfo.dimensionDescription"],
                    Height = form["truckInfo.height"],
                    Width = form["truckInfo.width"],
                    Length = form["truckInfo.length"],
                    FrontOverhang = form["truckInfo.frontOverhang"],
                    RearOverhang = form["truckInfo.rearOverhang"],
                    LeftOverhang = form["truckInfo.leftOverhang"],
                    RightOverhang = form["truckInfo.rightOverhang"],
                    Diagram = form["truckInfo.diagram"],
                    WeightPerAxle = form["truckInfo.weightPerAxle"],
                    AxleCount = form["truckInfo.axleCount"],
                    AxleLength = form["truckInfo.axleLength"],
                    MaxAxleWidth = form["truckInfo.maxAxleWidth"],
                    MaxAxleWeight = form["truckInfo.maxAxleWeight"],
                    TotalAxleWeight = form["truckInfo.totalAxleWeight"],
                    AxleGroupTireType = form["truckInfo.axleGroupTireType"],
                },
                Trailer = new TrailerInfo
                {
                    Make = form["trailerInfo.make"],
                    Type = form["trailerInfo.type"],
                    LicenseNumber = form["trailerInfo.licenseNumber"],
                    State = form["trailerInfo.state"],
                    EmptyWeight = form["trailerInfo.emptyWeight"],
                    WeightPerAxle = form["trailerInfo.weightPerAxle"],
                    AxleCount = form["trailerInfo.axleCount"],
                    AxleLength = form["trailerInfo.axleLength"],
                    MaxAxleWidth = form["trailerInfo.maxAxleWidth"],
                    MaxAxleWeight = form["trailerInfo.maxAxleWeight"],
                    TotalAxleWeight = form["trailerInfo.totalAxleWeight"],
                    AxleGroupTireType = form["trailerInfo.axleGroupTireType"],
                },
                Load = new LoadInfo
                {
                    Description = form["loadInfo.description"]
                },
                Movement = new MovementInfo
                {
                    StartDate = form["movementInfo.startDate"],
                    EndDate = form["movementInfo.endDate"],
                    HaulingHours = form["movementInfo.haulingHours"],
                    Origin = form["movementInfo.origin"],
                    Destination = form["movementInfo.destination"],
                    RouteDescription = form["movementInfo.routeDescription"],
                    RouteCountyNumbers = form["movementInfo.routeCountyNumbers"],
                    MilesOfCountyRoad = form["movementInfo.milesOfCountyRoad"],
                    RouteLength = form["movementInfo.routeLength"],
                    StateHighwayPermitNumber = form["movementInfo.stateHighwayPermitNumber"],
                    StateHighwayPermitIssued = form["movementInfo.stateHighwayPermitIssued"],
                    NeedPilotCar = form["movementInfo.needPilotCar"],
                    DestinationWithinCityLimits = form["movementInfo.destinationWithinCityLimits"],
                    DestinationWithinApplyingCounty = form["movementInfo.destinationWithinApplyingCounty"]
                },
                Route = form["route"],
                Status = form["status"]
            };
        }
    }

    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class PermitModule : NancyModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Func<NancyContext, Database, HostConfigurationSection, string> EvaluatePermit = MockPermitEvaluation;

        public PermitModule(Database database, HostConfigurationSection config) : base("/api/v1/issue")
        {
            // Need a valid JWT to access
            //this.RequiresAuthentication();

            // Accept a new application from the user
            Post["/"] = _ => ProcessApplication(database, config);

            Get["/{id}"] = _ => View["Permit", FetchPermit(database, config, Context.CurrentUser as AuthUser, int.Parse(_.id))];
            Post["/{id}"] = _ => UpdateApplication(database, config, Context.CurrentUser as AuthUser, _.id);
        }

        private Response UpdateApplication(Database database, HostConfigurationSection config, AuthUser user, int id)
        {
            // Configuration parameters
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);
            var issuerId = config.Keyword(Keys.SELF__IDENTIFIER);

            // Find the record
            var bundle = database.ApplicationById(id);

            // Generate the repository path
            var repoPath = Path.Combine(workspace, bundle.Application.Id);

            // If the repository does not already exists, clone it
            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(bundle.Application.Links.Origin, repoPath);
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
            // Convert the form into a PermitApplicationRecord
            var permitForm = Context.ToPermitDataRecord();

            var authorities = permit.SelectToken("data.attributes.authorities");
            var mySection = authorities.SelectToken(issuerId);
            var formSection = permit.SelectToken("data.attributes.form-data");
            if (mySection == null)
            {
                (permit.SelectToken("data.attributes.authorities") as JObject).Add(issuerId, new JObject());
                mySection = authorities.SelectToken(issuerId);
            }
            permit.SelectToken("data.attributes")["route"] = (JObject.Parse(permitForm.Route));
            mySection["route"] = JObject.Parse(permitForm.Route);
            mySection["status"] = PermitStatus.Text(int.Parse(permitForm.Status));
            mySection["reviewed"] = DateTime.Now;

            formSection["movementInfo.startDate"] = permitForm.Movement.StartDate;
            formSection["movementInfo.endDate"] = permitForm.Movement.EndDate;
            formSection["movementInfo.haulingHours"] = permitForm.Movement.HaulingHours;
            formSection["movementInfo.origin"] = permitForm.Movement.Origin;
            formSection["movementInfo.destination"] = permitForm.Movement.Destination;
            formSection["movementInfo.routeDescription"] = permitForm.Movement.RouteDescription;
            formSection["movementInfo.routeCountyNumbers"] = permitForm.Movement.RouteCountyNumbers;
            formSection["movementInfo.milesOfCountyRoad"] = permitForm.Movement.MilesOfCountyRoad;
            formSection["movementInfo.routeLength"] = permitForm.Movement.RouteLength;
            formSection["movementInfo.stateHighwayPermitNumber"] = permitForm.Movement.StateHighwayPermitNumber;
            formSection["movementInfo.stateHighwayPermitIssued"] = permitForm.Movement.StateHighwayPermitIssued;

            // Serialize all of the information into a JSON string
            var json = permit.ToJson();

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
            // Add the permit to the database
            database.UpdateApplication(json, id, Context.Request.Form["status"]);

            return new Nancy.Responses.RedirectResponse("../../../../permits/");
        }

        private static string MockPermitEvaluation(NancyContext context, Database database, HostConfigurationSection config)
        {            
            try
            {
                // Convert the form into a PermitApplicationRecord
                var permit = context.ToPermitApplicationRecord();

                // Serialize all of the information into a JSON string
                var json = permit.ToJson();

                // Add the permit to the database
                database.CreatePermitForUser(context.CurrentUser, json);
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to create permit application");
                return PermitApprovalStatus.DENIED;
            }

            return PermitApprovalStatus.UNDER_REVIEW;
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

        private static string DefaultPermitEvaluation(NancyContext context, Database database, HostConfigurationSection config)
        {
            switch (WebModule.Configuration.Behavior)
            {
                case "always": return PermitApprovalStatus.APPROVED;
                case "random": return PermitApprovalStatus.RANDOM;
                case "na": return PermitApprovalStatus.NO_AUTHORITY;
                case "no_authority": return PermitApprovalStatus.NO_AUTHORITY;
                case "under_review": return PermitApprovalStatus.UNDER_REVIEW;
                default:
                case "never": return PermitApprovalStatus.DENIED;
            }
        }

        private Response ProcessApplication(Database database, HostConfigurationSection config)
        {
            var identifier = config.Keyword(Keys.SELF__IDENTIFIER);
            var status = EvaluatePermit(Context, database, config);

            return Response.AsJson(new PermitApprovalRecord
            {
                Timestamp = DateTime.Now,
                Status = status,
                Authority = identifier
            });
        }

        private PermitApplication FetchPermit(Database services, HostConfigurationSection config, AuthUser user, int permitIdentifier)
        {
            // Configuration parameters
            var workspace = config.Keyword(Keys.UPP__PERMIT_WORKSPACE);
            var repoUrlTemplate = config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE);

            // Find the record
            var bundle = services.ApplicationById(permitIdentifier); //.FetchPermitBundle(user.ExtendedClaims["upp"] as string, repoUrlTemplate, permitIdentifier);

            // Generate the repository path
            var repoPath = Path.Combine(workspace, bundle.Application.Id);

            // If the repository does not already exists, clone it
            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(bundle.Application.Links.Origin, repoPath);
            }

            using (var repo = new Repository(repoPath))
            {
                // Update the checkout
                var author = new Signature(user.UserName, user.Email, DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, new PullOptions());

                // Read the current permit file and return
                var permitPath = Path.Combine(repo.Info.WorkingDirectory, "permit.json");
                var permit = JObject.Parse(File.ReadAllText(permitPath));
                var form = permit.SelectToken("data.attributes.form-data");
                var hauler = form["haulerInfo.name"];
                string name = hauler.Value<string>();
                bundle.PermitData = new PermitDataRecord();
                bundle.PermitData.Hauler.Name = form["haulerInfo.name"].Value<string>();
                bundle.PermitData.Hauler.Date = form["haulerInfo.date"].Value<DateTime>();
                bundle.PermitData.Hauler.Email = form["haulerInfo.email"].Value<string>();
                bundle.PermitData.Hauler.Phone = form["haulerInfo.phone"].Value<string>();
                bundle.PermitData.Hauler.Fax = form["haulerInfo.fax"].Value<string>();
                bundle.PermitData.Company.Name = form["companyInfo.name"].Value<string>();
                bundle.PermitData.Company.Address = form["companyInfo.address"].Value<string>();
                bundle.PermitData.Company.Email = form["companyInfo.email"].Value<string>();
                bundle.PermitData.Company.Contact = form["companyInfo.contact"].Value<string>();
                bundle.PermitData.Company.Phone = form["companyInfo.phone"].Value<string>();
                bundle.PermitData.Company.Fax = form["companyInfo.fax"].Value<string>();
                bundle.PermitData.Insurance.Provider = form["insuranceInfo.provider"].Value<string>();
                bundle.PermitData.Insurance.AgencyAddress = form["insuranceInfo.agencyAddress"].Value<string>();
                bundle.PermitData.Insurance.PolicyNumber = form["insuranceInfo.policyNumber"].Value<string>();
                bundle.PermitData.Vehicle.Make = form["vehicleInfo.make"].Value<string>();
                bundle.PermitData.Vehicle.Type = form["vehicleInfo.type"].Value<string>();
                bundle.PermitData.Vehicle.License = form["vehicleInfo.license"].Value<string>();
                bundle.PermitData.Vehicle.State = form["vehicleInfo.state"].Value<string>();
                bundle.PermitData.Vehicle.SerialNumber = form["vehicleInfo.serialNumber"].Value<string>();
                bundle.PermitData.Vehicle.USDOTNumber = form["vehicleInfo.USDOTNumber"].Value<string>();
                //form["vehicleInfo.emptyWeight"].Value<string>();

                bundle.PermitData.Truck.GrossWeight = form["truckInfo.grossWeight"].Value<decimal>();
                bundle.PermitData.Truck.DimensionSummary = form["truckInfo.dimensionSummary"].Value<string>();
                bundle.PermitData.Truck.DimensionDescription = form["truckInfo.dimensionDescription"].Value<string>();
                bundle.PermitData.Truck.Height = form["truckInfo.height"].Value<decimal>();
                bundle.PermitData.Truck.Width = form["truckInfo.width"].Value<decimal>();
                bundle.PermitData.Truck.Length = form["truckInfo.length"].Value<decimal>();
                bundle.PermitData.Truck.FrontOverhang = form["truckInfo.frontOverhang"].Value<decimal>();
                bundle.PermitData.Truck.RearOverhang = form["truckInfo.rearOverhang"].Value<decimal>();
                bundle.PermitData.Truck.LeftOverhang = form["truckInfo.leftOverhang"].Value<decimal>();
                bundle.PermitData.Truck.RightOverhang = form["truckInfo.rightOverhang"].Value<decimal>();
                //form["truckInfo.diagram"].Value<string>();
                //form["truckInfo.weightPerAxle"].Value<string>();
                //form["truckInfo.axleCount"].Value<string>();
                //form["truckInfo.axleLength"].Value<string>();
                //form["truckInfo.maxAxleWidth"].Value<string>();
                //form["truckInfo.maxAxleWeight"].Value<string>();
                //form["truckInfo.totalAxleWeight"].Value<string>();
                //form["truckInfo.axleGroupTireType"].Value<string>();
                bundle.PermitData.Trailer.Make =  form["trailerInfo.make"].Value<string>();
                bundle.PermitData.Trailer.Type = form["trailerInfo.type"].Value<string>();
                bundle.PermitData.Trailer.LicenseNumber = form["trailerInfo.licenseNumber"].Value<string>();
                bundle.PermitData.Trailer.State = form["trailerInfo.state"].Value<string>();
                //form["trailerInfo.emptyWeight"].Value<string>();
                bundle.PermitData.Trailer.WeightPerAxle =  form["trailerInfo.weightPerAxle"].Value<decimal>();
                //form["trailerInfo.axleCount"].Value<string>();
                //form["trailerInfo.axleLength"].Value<string>();
                //form["trailerInfo.maxAxleWidth"].Value<string>();
                //form["trailerInfo.maxAxleWeight"].Value<string>();
                bundle.PermitData.Trailer.TotalAxleWeight = form["trailerInfo.totalAxleWeight"].Value<decimal>();
                //form["trailerInfo.axleGroupTireType"].Value<string>();
                bundle.PermitData.Load.Description = form["loadInfo.description"].Value<string>();
                bundle.PermitData.Movement.StartDate = form["movementInfo.startDate"].Value<DateTime>();
                bundle.PermitData.Movement.EndDate = form["movementInfo.endDate"].Value<DateTime>();
                bundle.PermitData.Movement.HaulingHours = form["movementInfo.haulingHours"].Value<decimal>();
                bundle.PermitData.Movement.Origin = form["movementInfo.origin"].Value<string>();
                bundle.PermitData.Movement.Destination = form["movementInfo.destination"].Value<string>();
                bundle.PermitData.Movement.RouteDescription = form["movementInfo.routeDescription"].Value<string>();
                //bundle.PermitData.Movement.RouteCountyNumbers = form["movementInfo.routeCountyNumbers"].Value<string>();
                //bundle.PermitData.Movement.MilesOfCountyRoad = form["movementInfo.milesOfCountyRoad"].Value<string>();
                bundle.PermitData.Movement.RouteLength = form["movementInfo.routeLength"].Value<decimal>();
                bundle.PermitData.Movement.StateHighwayPermitNumber = form["movementInfo.stateHighwayPermitNumber"].Value<string>();
                bundle.PermitData.Movement.StateHighwayPermitIssued = form["movementInfo.stateHighwayPermitIssued"].Value<string>();

                bundle.PermitData.Route = permit.SelectToken("data.attributes.route").ToJson();
                //return bundle; //JObject.Parse(File.ReadAllText(permit));
            }
            return bundle;
        }
    }
 }
