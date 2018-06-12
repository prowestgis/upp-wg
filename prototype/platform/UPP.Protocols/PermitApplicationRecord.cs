using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UPP.Protocols
{
    public interface IPermitApplicationRecord
    {
        HaulerInfo Hauler { get; }
        CompanyInfo Company { get; }
        InsuranceInfo Insurance { get;  }
        VehicleInfo Vehicle { get; }
        TruckInfo Truck { get; }
        AxleInfo Axle { get; }
        TrailerInfo Trailer { get; }
        LoadInfo Load { get; }
        MovementInfo Movement { get; }
    }

    public sealed class PermitApplicationRecord : IPermitApplicationRecord
    {
        public PermitApplicationRecord()
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
        }

        public HaulerInfo Hauler { get; set; }
        public CompanyInfo Company { get; set; }
        public InsuranceInfo Insurance { get; set; }
        public VehicleInfo Vehicle { get; set; }
        public TruckInfo Truck { get; set; }
        public AxleInfo Axle { get; set; }
        public TrailerInfo Trailer { get; set; }
        public LoadInfo Load { get; set; }
        public MovementInfo Movement { get; set; }

        public string Route { get; set; }
    }

    // All of the fields from the UPP working group
    public sealed class HaulerInfo
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime Date { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Phone { get; set; }

        [JsonProperty(Required = Required.DisallowNull)]
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
        public decimal MilesOfCountyRoad { get; set; }
        public decimal RouteLength { get; set; }

        public string StateHighwayPermitNumber { get; set; }
        public string StateHighwayPermitIssued { get; set; }
        public bool NeedPilotCar { get; set; }
        public bool DestinationWithinCityLimits { get; set; }
        public bool DestinationWithinApplyingCounty { get; set; }
    }
}
