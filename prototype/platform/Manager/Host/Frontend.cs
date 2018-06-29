using Nancy;
using Dapper;
using Manager.Store;
using Nancy.ModelBinding;
using Nancy.Security;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Security;
using Nancy.Cookies;
using NLog;
using UPP.Security;
using UPP.Configuration;
using UPP.Protocols;
using static Manager.Store.Services;

namespace Manager.Host
{
    public sealed class Frontend : NancyModule
    {
        private readonly AuthSettings _authSettings;

        public Frontend(Services services, AuthSettings authSettings, HostConfigurationSection config)
        {
            _authSettings = authSettings;

            Get["/"] = _ => View["Index", new DashboardView(services, config, Context)];
            Get["/dashboard.html"] = _ => View["Index", new DashboardView(services, config, Context)];            
            Get["/authentication/logout"] = _ => Logout(config.Keyword(Keys.NANCY__HOST_BASE_URI));            
        }

        public Response Logout(string baseUri)
        {
            // Reset the token to an empty string and set an very old expiration date
            var cookie = new NancyCookie(_authSettings.CookieName, String.Empty, DateTime.MinValue);
            return Response.AsRedirect(baseUri).WithCookie(cookie);
        }
    }

    public sealed class PermitApplication : NancyModule
    {
        public PermitApplication(Services services)
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { Claims.HAULER });

            Get["/permit.html"] = _ => View["Permit", new PermitView(services, Context)];
        }
    }

    public sealed class AdministrativeInterface : NancyModule
    {
        public AdministrativeInterface(Services services, HostConfigurationSection config)
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { Claims.UPP_ADMIN });

            Get["/admin.html"] = _ => View["Administration", new AdministrativeView { Users = services.AllUsers() }];
            Get["/users/{id}/edit"] = _ => View["EditUser", services.AllUsers().FirstOrDefault(x => x.UserId == _.id)];
            Post["/users/{id}/edit"] = _ => UpdateUser(services, config.Keyword(Keys.NANCY__HOST_BASE_URI));
        }

        private Response UpdateUser(Services services, string baseUri)
        {
            var user = this.Bind<Manager.Store.Services.UserRecord>();
            user = services.UpdateIdentityRecord(user);

            return Response.AsRedirect(String.Format("{0}users/{1}/edit", baseUri, user.UserId));
        }
    }

    public sealed class AdministrativeView
    {
        public List<Services.UserRecord> Users { get; set; }
    }

    public sealed class DashboardView
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DashboardView(Services services, HostConfigurationSection config, NancyContext context)
        {
            ServiceDirectoryUrl = config.Keyword(Keys.SERVICE_DIRECTORY__BASE_URI);
            Logins = services.AuthenticationProviders.Select(x => new PrimaryLogin
            {
                Name = x.DisplayName,
                Url = String.Format("/authentication/redirect/{0}", x.Name),
                Image = String.Format("/images/{0}-login-button.png", x.Name)
            }).ToList();

            User = context.CurrentUser as AuthUser;

            logger.Debug("Dashboard user is {0}", User == null ? "NULL" : User.UserName);

            // List the active OAuth credentials and registered microservices
            Auths = services.AuthenticationProviders.ToList();
            //MicroServices = services.MicroServices.ToList();

            var baseUrl = config.Keyword(Keys.NANCY__HOST_BASE_URI);
            if (User == null)
            {
                Permits = Enumerable.Empty<PermitBundle>().ToList();
            }
            else
            {
                Permits = services
                    .FindPermitBundles(User.ExtendedClaims["upp"] as string, config.Keyword(Keys.UPP__PERMIT_REPOSITORY_URL_TEMPLATE))
                    .Select(x => { x.RepositoryName = String.Format("{0}api/permits/{1}", baseUrl, x.PermitId); return x; })
                    .ToList();
            }
        }

        public string ServiceDirectoryUrl { get; set; }
        public AuthUser User { get; set; }
        public List<PrimaryLogin> Logins { get; }
        public List<Services.OAuthProvider> Auths { get; }
        public List<PermitBundle> Permits { get; }

        //public List<Services.MicroServiceProviderConfig> MicroServices { get; }

        public sealed class PrimaryLogin
        {
            public string Url { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
        }
    }

    public static class ManagerHelpers
    {
        public static HaulerInfo ToHaulerInfo(this AuthUser user)
        {
            return new HaulerInfo
            {
                Date = DateTime.Now,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
            };
        }
    }

    public sealed class PermitView
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PermitView(Services services, NancyContext context)
        {
            User = context.CurrentUser as AuthUser;

            Hauler = User.ToHaulerInfo();
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
    }
}
