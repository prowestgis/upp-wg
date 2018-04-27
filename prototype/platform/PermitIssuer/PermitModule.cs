using Nancy;
using Nancy.Security;
using Nancy.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPP.Configuration;
using UPP.Protocols;
using UPP.Security;

namespace PermitIssuer
{

    public static class PermitHelpers
    {
        public static string ToJson(this object item)
        {
            using (var writer = new StringWriter())
            {
                var serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(writer, item);
                return writer.ToString();
            }
        }

        public static IPermitApplicationRecord ToPermitApplicationRecord(this NancyContext context)
        {
            var form = context.Request.Form;

            return new PermitApplicationRecord
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
                }
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
            this.RequiresAuthentication();

            // Show all of the user's applications
            Get["/"] = _ => ShowApplications(database);

            // Accept a new application from the user
            Post["/"] = _ => ProcessApplication(database, config);
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

        private static string DefaultPermitEvaluation(NancyContext context, Database database, HostConfigurationSection config)
        {
            switch (WebModule.Configuration.Behavior)
            {
                case "always": return PermitApprovalStatus.APPROVED;
                case "random": return PermitApprovalStatus.RANDOM;
                case "na": return PermitApprovalStatus.NO_AUTHORITY;
                default:
                case "never": return PermitApprovalStatus.DENIED;
            }
        }

        private Response ShowApplications(Database database)
        {
            // Look up the permits for this user
            var permits = database.FindPermitsForUser(Context.CurrentUser);
            var serializer = JsonSerializer.CreateDefault();
            foreach (var permit in permits)
            {
                if (permit.Data is string)
                {
                    permit.Data = serializer.Deserialize(new StringReader(permit.Data), typeof(object));
                }
            }
            return Response.AsJson(permits);
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
    }
}
