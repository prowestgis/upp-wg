using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Protocols
{
    public sealed class ServiceRegistrationType
    {
        public static readonly ServiceRegistrationType ROUTE = new ServiceRegistrationType(Domain.ROUTE, "route", "Routing Services");
        public static readonly ServiceRegistrationType GEOMETRY = new ServiceRegistrationType(Domain.GEOMETRY, "geometry", "Low-Level Geometry Operations");
        public static readonly ServiceRegistrationType COUNTY_BOUNDARIES = new ServiceRegistrationType(Domain.COUNTY_BOUNDARIES, "boundaries.county", "Official County Boundaries");
        public static readonly ServiceRegistrationType CITY_BOUNDARIES = new ServiceRegistrationType(Domain.CITY_BOUNDARIES, "boundaries.city", "Official City Boundaries");
        public static readonly ServiceRegistrationType UPP = new ServiceRegistrationType(Domain.UPP, "upp", "UPP Trusted Service Endpoint");

        public static readonly ServiceRegistrationType UPP_INFORMATION_AXLE = new ServiceRegistrationType(Domain.UPP_INFORMATION_AXLE, "upp.information.axle", "UPP Axle Information");
        public static readonly ServiceRegistrationType UPP_INFORMATION_COMPANY = new ServiceRegistrationType(Domain.UPP_INFORMATION_COMPANY, "upp.information.company", "UPP Company Information");
        public static readonly ServiceRegistrationType UPP_INFORMATION_INSURANCE = new ServiceRegistrationType(Domain.UPP_INFORMATION_INSURANCE, "upp.information.insurance", "UPP Insurance Information");
        public static readonly ServiceRegistrationType UPP_INFORMATION_TRAILER = new ServiceRegistrationType(Domain.UPP_INFORMATION_TRAILER, "upp.information.trailer", "UPP Trailer Information");
        public static readonly ServiceRegistrationType UPP_INFORMATION_TRUCK = new ServiceRegistrationType(Domain.UPP_INFORMATION_TRUCK, "upp.information.truck", "UPP Truck Information");
        public static readonly ServiceRegistrationType UPP_INFORMATION_VEHICLE = new ServiceRegistrationType(Domain.UPP_INFORMATION_VEHICLE, "upp.information.vehicle", "UPP Vehicle Information");

        public enum Domain
        {
            ROUTE,
            GEOMETRY,
            COUNTY_BOUNDARIES,
            CITY_BOUNDARIES,
            UPP,
            UPP_INFORMATION_AXLE,
            UPP_INFORMATION_COMPANY,
            UPP_INFORMATION_INSURANCE,
            UPP_INFORMATION_TRAILER,
            UPP_INFORMATION_TRUCK,
            UPP_INFORMATION_VEHICLE
        };

        private static readonly List<ServiceRegistrationType> domain = new List<ServiceRegistrationType>
        {
            ROUTE,
            GEOMETRY,
            COUNTY_BOUNDARIES,
            CITY_BOUNDARIES,
            UPP,
            UPP_INFORMATION_AXLE,
            UPP_INFORMATION_COMPANY,
            UPP_INFORMATION_INSURANCE,
            UPP_INFORMATION_TRAILER,
            UPP_INFORMATION_TRUCK,
            UPP_INFORMATION_VEHICLE
        };

        public static IEnumerable<ServiceRegistrationType> AsEnumerable()
        {
            return domain.AsReadOnly();
        }

        private ServiceRegistrationType(Domain value, string key, string displayText)
        {
            Value = value;
            Key = key;
            DisplayText = displayText;
        }

        public Domain Value { get; private set; }
        public string Key { get; private set; }
        public string DisplayText { get; private set; }

        // Allow explicit conversion from a string in order to gain type safety
        public static explicit operator ServiceRegistrationType(string value)
        {
            return domain.Single(x => x.Key == value);
        }
    }
}
