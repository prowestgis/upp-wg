using Nancy;
using Dapper;
using Manager.Store;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Security;
using Nancy.Cookies;
using NLog;

namespace Manager.Host
{
    public sealed class Frontend : NancyModule
    {
        private readonly AuthSettings _authSettings;

        public Frontend(Services services, AuthSettings authSettings)
        {
            _authSettings = authSettings;

            Get["/"] = _ => View["Index", new DashboardView(services, Context)];
            Get["/dashboard.html"] = _ => View["Index", new DashboardView(services, Context)];
            Get["/permit.html"] = _ => View["Permit", new PermitView(services, Context)];
            Get["/authentication/logout"] = _ => Logout();
        }

        public Response Logout()
        {
            // Reset the token to an empty string and set an very old expiration date
            var cookie = new NancyCookie(_authSettings.CookieName, String.Empty, DateTime.MinValue);
            return Response.AsRedirect("/").WithCookie(cookie);
        }
    }

    public sealed class DashboardView
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DashboardView(Services services, NancyContext context)
        {
            Logins = services.AuthenticationProviders.Select(x => new PrimaryLogin
            {
                Name = x.Name,
                Url = String.Format("/authentication/redirect/{0}", x.Name),
                Image = String.Format("/images/{0}-login-button.png", x.Name)
            }).ToList();

            User = context.CurrentUser as AuthUser;

            logger.Debug("Dashboard user is {0}", User == null ? "NULL" : User.UserName);

            // List the active OAuth credentials and registered microservices
            Auths = services.AuthenticationProviders.ToList();
            MicroServices = services.MicroServices.ToList();
        }

        public AuthUser User { get; set; }
        public List<PrimaryLogin> Logins { get; }
        public List<Services.OAuthProviderConfig> Auths { get; }
        public List<Services.MicroServiceProviderConfig> MicroServices { get; }

        public sealed class PrimaryLogin
        {
            public string Url { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
        }
    }

    public sealed class PermitView
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PermitView(Services services, NancyContext context)
        {
            User = context.CurrentUser as AuthUser;

            Hauler = new HaulerInfo();
            Company = new CompanyInfo();
            Insurance = new InsuranceInfo();
            Vehicle = new VehicleInfo();
            Truck = new TruckInfo();
            Axle = new AxleInfo();
            Trailer = new TrailerInfo();
            Load = new LoadInfo();
            Movement = new MovementInfo();
        }

        public AuthUser User { get; }
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
            public decimal FrontOverhang { get; set; }
            public decimal RearOverhang { get; set; }
            public decimal LeftOverhang { get; set; }
            public decimal RightOverhang { get; set; }
            public string Diagram { get; set; }
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
            public decimal AxleGroupTotalWidth { get; set; }
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
            public decimal MileOfCountyRoad { get; set; }
            public decimal RouteLength { get; set; }
            public bool NeedPilotCar { get; set; }
        }
    }
}
