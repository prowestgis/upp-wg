using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UPP.Configuration;
using UPP.Protocols;

namespace PermitIssuer
{
    /// <summary>
    /// The agent API allows trusted hosts to manage their metadata
    /// </summary>
    public sealed class PermitModule : NancyModule
    {
        private Func<NancyContext, string> EvaluatePermit = DefaultPermitEvaluation;

        public PermitModule(HostConfigurationSection config, Database database) : base("/permits")
        {
            // Need a valid JWT to access
            //this.RequiresAuthentication();

            Get["/admin.html"] = _ => View["Administration", new AdministrativeView { Permits = database.AllPermits() }];
            Get["/{id}/edit"] = _ => View["Permit", new PermitView(database, _.id)];
            //Post["/{id}/edit"] = _ => UpdateUser(services, config.Keyword(Keys.NANCY__HOST_BASE_URI));
            Post["/"] = _ => ProcessApplication(config, database);
        }

        public sealed class AdministrativeView
        {
            public List<Database.PermitRecord> Permits { get; set; }
        }

        private static string DefaultPermitEvaluation(NancyContext context)
        {
            switch (WebModule.Configuration.Behavior)
            {
                case "always": return PermitApprovalStatus.APPROVED;
                case "random": return PermitApprovalStatus.RANDOM;
                case "no_authority": return PermitApprovalStatus.NO_AUTHORITY;
                case "under_review": return PermitApprovalStatus.UNDER_REVIEW;
                default:
                case "never": return PermitApprovalStatus.DENIED;
            }
        }

        private Response ProcessApplication(HostConfigurationSection config, Database db)
        {
            var identifier = config.Keyword(Keys.SELF__IDENTIFIER);
            var status = EvaluatePermit(Context);
            var record = this.Bind<PostedPermitModel>();

            db.CreateNewPermit(record, status);

            return Response.AsJson(new PermitApprovalRecord
            {
                Timestamp = DateTime.Now,
                Status = status,
                Authority = identifier
            });
        }
        public sealed class PostedPermitModel
        {
            public string haulerinfoName { get; set; }
            public DateTime haulerinfoDate { get; set; }
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
            //public string axleinfoDescription { get; set; }
            //public string axleinfoWeightPerAxle { get; set; }
            //public string axleinfoDescriptionSummary { get; set; }
            //public int axleinfoAxleCount { get; set; }
            //public int axleinfoGroupCount { get; set; }
            //public string axleinfoApproxAxleLength { get; set; }
            //public string axleinfoAxleLength { get; set; }
            //public string axleinfoMaxAxleWidth { get; set; }
            //public string axleinfoMaxAxleWeight { get; set; }
            //public string axleinfoTotalAxleWeight { get; set; }
            //public string axleinfoAxleGroupSummary { get; set; }
            //public string axleinfoAxelsPerGroup { get; set; }
            //public string axleinfoAxleGroupTireType { get; set; }
            //public string axleinfoAxleGroupWidth { get; set; }
            //public string axleinfoAxleOperatingWeights { get; set; }
            //public string axleinfoAxleGroupWeight { get; set; }
            //public string axleinfoAxleGroupMaxWidth { get; set; }
            //public string axleinfoAxleGroupTotalWeight { get; set; }
            //public string axleinfoAxleGroupDistance { get; set; }
            //public string trailerinfoDescription { get; set; }
            public string trailerinfoMake { get; set; }
            //public string trailerinfoModel { get; set; }
            public string trailerinfoType { get; set; }
            //public string trailerinfoSerialNumber { get; set; }
            public string trailerinfoLicenseNumber { get; set; }
            public string trailerinfoState { get; set; }
            public string trailerinfoEmptyWeight { get; set; }
            //public string trailerinfoRegisteredWeight { get; set; }
            //public string trailerinfoRegulationWeight { get; set; }
            //public string loadinfoOwner { get; set; }
            //public string loadinfoOverSize { get; set; }
            //public string loadinfoOverWeight { get; set; }
            public string loadinfoDescription { get; set; }
            //public string loadinfoSizeOrModel { get; set; }
            //public string loadinfoWeight { get; set; }

            public DateTime? movementinfoStartDate { get; set; }
            public DateTime? movementinfoEndDate { get; set; }
            public string movementinfoHaulingHours { get; set; }
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

            //public string[] Authority { get; set; }

            //public string[] Bridges { get; set; }

         }
    }
    public sealed class PermitView
    {
        public PermitView(Database db, string id)
        {
         
            Hauler = new HaulerInfo();
            Company = new CompanyInfo();
            Insurance = new InsuranceInfo();
            Vehicle = new VehicleInfo();
            Truck = new TruckInfo();
            Axle = new AxleInfo();
            Trailer = new TrailerInfo();
            Load = new LoadInfo();
            Movement = new MovementInfo();

            var permit = db.AllPermits().FirstOrDefault();
            if(permit != null)
            {
                Hauler.Name = permit.HaulerName;
                Hauler.Date = permit.ApplicationDate;
                Hauler.Email = permit.HaulerEmail;
                Hauler.Fax = permit.HaulerFax;
                Hauler.Phone = permit.HaulerPhone;

                Company.Name = permit.CompanyName;
                Company.Address = permit.CompanyAddress;
                Company.Email = permit.CompanyEmail;
                Company.Contact = permit.CompanyContact;
                Company.Phone = permit.CompanyPhone;
                Company.Fax = permit.CompanyFax;

                Insurance.Provider = permit.InsuranceProvider;
                Insurance.AgencyAddress = permit.InsuranceAgencyAddress;
                Insurance.PolicyNumber = permit.InsurancePolicyNumber;

                Vehicle.Make = permit.VehicleMake;
                Vehicle.Type = permit.VehicleType;
                Vehicle.License = permit.VehicleLicenseNumber;
                Vehicle.State = permit.VehicleState;
                Vehicle.USDOTNumber = permit.VehicleUsdotNumber;
                Vehicle.EmptyWeight = permit.VehicleEmptyWeight;

                Truck.TotalAxleWeight = permit.TotalGrossWeight;
                Truck.Height = permit.Height;
                Truck.Width = permit.Width;
                Truck.Length = permit.CombinedLength;
                Truck.FrontOverhang = permit.OverhangFront;
                Truck.RearOverhang = permit.OverhangRear;
                Truck.LeftOverhang = permit.OverhangLeft;
                Truck.RightOverhang = permit.OverhangRight;

                Trailer.Make = permit.TrailerMake;
                Trailer.Type = permit.TrailerType;
                Trailer.LicenseNumber = permit.TrailerLicenseNumber;
                Trailer.State = permit.TrailerState;
                Trailer.EmptyWeight = permit.TrailerEmptyWeight;

                Load.Description = permit.LoadDescription;


            }
        }

        //public AuthUser User { get; }
        public HaulerInfo Hauler { get; }
        public CompanyInfo Company { get; }
        public InsuranceInfo Insurance { get; }
        public VehicleInfo Vehicle { get; }
        public TruckInfo Truck { get; }
        public AxleInfo Axle { get; }
        public TrailerInfo Trailer { get; }
        public LoadInfo Load { get; }
        public MovementInfo Movement { get; }

        // All of the fields from the UPP working group
        public sealed class HaulerInfo
        {
            public HaulerInfo()
            {
                Date = DateTime.Now;
            }

            public string Name { get; set; }
            public DateTime Date { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
        }

        public sealed class CompanyInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Contact { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string Cell { get; set; }
            public string BillTo { get; set; }
            public string BillingAddress { get; set; }
        }

        public sealed class InsuranceInfo
        {
            public string Provider { get; set; }
            public string AgencyAddress { get; set; }
            public string PolicyNumber { get; set; }
            public decimal InsuredAmount { get; set; }
        }

        public sealed class VehicleInfo
        {
            public string Year { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Type { get; set; }
            public string License { get; set; }
            public string State { get; set; }
            public string SerialNumber { get; set; }
            public string USDOTNumber { get; set; }
            public decimal EmptyWeight { get; set; }
            public decimal RegisteredWeight { get; set; }
        }

        public sealed class TruckInfo
        {
            public decimal GrossWeight { get; set; }
            public decimal EmptyWeight { get; set; }
            public decimal RegisteredWeight { get; set; }
            public decimal RegulationWeight { get; set; }
            public string DimensionSummary { get; set; }
            public string DimensionDescription { get; set; }
            public decimal Height { get; set; }
            public decimal Width { get; set; }
            public decimal Length { get; set; }
            public decimal FrontOverhang { get; set; }
            public decimal RearOverhang { get; set; }
            public decimal LeftOverhang { get; set; }
            public decimal RightOverhang { get; set; }
            public string Diagram { get; set; }

            // Refactored axle information
            public decimal WeightPerAxle { get; set; }
            public int AxleCount { get; set; }
            public decimal AxleLength { get; set; }
            public decimal MaxAxleWidth { get; set; }
            public decimal MaxAxleWeight { get; set; }
            public decimal TotalAxleWeight { get; set; }
            public string AxleGroupTireType { get; set; }
        }

        public sealed class AxleInfo
        {
            public string Description { get; set; }
            public decimal WeightPerAxle { get; set; }
            public string DescriptionSummary { get; set; }
            public int AxleCount { get; set; }
            public int GroupCount { get; set; }
            public decimal ApproxAxleLength { get; set; }
            public decimal AxleLength { get; set; }
            public decimal MaxAxleWidth { get; set; }
            public decimal MaxAxleWeight { get; set; }
            public decimal TotalAxleWeight { get; set; }
            public string AxleGroupSummary { get; set; }
            public int AxelsPerGroup { get; set; }
            public string AxleGroupTireType { get; set; }
            public decimal AxleGroupWidth { get; set; }
            public decimal AxleOperatingWeights { get; set; }
            public decimal AxleGroupWeight { get; set; }
            public decimal AxleGroupMaxWidth { get; set; }
            public decimal AxleGroupTotalWeight { get; set; }
            public decimal AxleGroupDistance { get; set; }
        }

        public sealed class TrailerInfo
        {
            public string Description { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Type { get; set; }
            public string SerialNumber { get; set; }
            public string LicenseNumber { get; set; }
            public string State { get; set; }
            public decimal EmptyWeight { get; set; }
            public decimal RegisteredWeight { get; set; }
            public decimal RegulationWeight { get; set; }

            // Refactored axle information
            public decimal WeightPerAxle { get; set; }
            public int AxleCount { get; set; }
            public decimal AxleLength { get; set; }
            public decimal MaxAxleWidth { get; set; }
            public decimal MaxAxleWeight { get; set; }
            public decimal TotalAxleWeight { get; set; }
            public string AxleGroupTireType { get; set; }
        }

        public sealed class LoadInfo
        {
            public string Owner { get; set; }
            public bool OverSize { get; set; }
            public bool OverWeight { get; set; }
            public string Description { get; set; }
            public string SizeOrModel { get; set; }
            public decimal Weight { get; set; }
        }

        public sealed class MovementInfo
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal HaulingHours { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }

            public string RouteDescription { get; set; }
            public string RouteCountyNumbers { get; set; }
            public decimal MileOfCountyRoad { get; set; }
            public decimal RouteLength { get; set; }

            public string StateHighwayPermitNumber { get; set; }
            public string StateHighwayPermitIssued { get; set; }
            public bool NeedPilotCar { get; set; }
            public bool DestinationWithinCityLimits { get; set; }
            public bool DestinationWithinApplyingCounty { get; set; }
        }
    }

}
