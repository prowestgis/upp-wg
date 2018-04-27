using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace PermitIssuer
{
    public class Database : DataStore
    {
        public Database(HostConfigurationSection config)
            : base("PermitIssuer.sqlite", @"App_Data\Schema.sql", config)
        {
        }

        public sealed class PermitRecord
        {
            public string PermitId { get; set; }
            
            public string PermitStatus { get; set; }
            public DateTime ApplicationDate { get; set; }

            public string HaulerName { get; set; }
            public string HaulerEmail { get; set; }
            public string HaulerPhone { get; set; }
            public string HaulerFax { get; set; }

            public string CompanyName { get; set; }
            public string CompanyAddress { get; set; }
            public string CompanyEmail { get; set; }
            public string CompanyContact { get; set; }
            public string CompanyPhone { get; set; }
            public string CompanyFax { get; set; }
            public string InsuranceProvider { get; set; }
            public string InsuranceAgencyAddress { get; set; }
            public string InsurancePolicyNumber { get; set; }
            public string VehicleMake { get; set; }
            public string VehicleType { get; set; }
            public string VehicleLicenseNumber { get; set; }
            public string VehicleState { get; set; }
            public string VehicleTruckSerialNumber { get; set; }
            public string VehicleUsdotNumber { get; set; }
            public decimal VehicleEmptyWeight { get; set; }
            public string TrailerMake { get; set; }
            public string TrailerType { get; set; }
            public string TrailerLicenseNumber { get; set; }
            public string TrailerState { get; set; }
            public decimal TrailerEmptyWeight { get; set; }
            public decimal TotalGrossWeight { get; set; }
            public decimal Height { get; set; }
            public decimal Width { get; set; }
            public decimal CombinedLength { get; set; }
            public decimal OverhangFront { get; set; }
            public decimal OverhangRear { get; set; }
            public decimal OverhangLeft { get; set; }
            public decimal OverhangRight { get; set; }

            public string LoadDescription { get; set; }

        }

        public List<PermitRecord> AllPermits()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<PermitRecord>(@"
                    SELECT permit_id as PermitId, application_date as ApplicationDate, permit_status as PermitStatus, hauler_name as HaulerName, hauler_email as HaulerEmail, hauler_phone as HaulerPhone, 
                    hauler_fax as HaulerFax, company_name as CompanyName, company_address as CompanyAddress, company_email as CompanyEmail, company_contact as CompanyContact, company_phone as CompanyPhone, 
                    company_fax as CompanyFax, insurance_provider as InsuranceProvider, insurance_agency_address as InsuranceAgencyAddress, insurance_policy_number as InsurancePolicyNumber, vehicle_make as VehicleMake,
                    vehicle_type as VehicleType, vehicle_license_number as VehicleLicenseNumber, vehicle_state as VehicleState, vehicle_truck_serial_number as VehicleTruckSerialNumber, 
                    vehicle_usdot_number as VehicleUsdotNumber, vehicle_empty_weight as VehicleEmptyWeight, trailer_make as TrailerMake, trailer_type as TrailerType, trailer_license_number as TrailerLicenseNumber,
                    trailer_state as TrailerState, trailer_empty_weight as TrailerEmptyWeight, total_gross_weight as TotalGrossWeight, height as Height, width as Width, combined_length as CombinedLength,
                    overhang_front as OverhangFront, overhang_rear as OverhangRear, overhang_left as OverhangLeft, overhang_right as OverhangRight, load_description as LoadDescription
                    FROM Permits
                    "
                 ).ToList();
            }
        }

        public string CreateNewPermit(PermitModule.PostedPermitModel permit, string status)
        {
            using (var conn = SimpleDbConnection())
            {
                // Create a new User
                var guid = Guid.NewGuid().ToString();
                conn.Execute(@"
                    INSERT INTO Permits (permit_id, application_date, permit_status, hauler_name, hauler_email, hauler_phone, hauler_fax, company_name, company_address, company_email, company_contact, company_phone, company_fax, insurance_provider, insurance_agency_address, insurance_policy_number, vehicle_make, vehicle_type, vehicle_license_number, vehicle_state, vehicle_truck_serial_number, vehicle_usdot_number, vehicle_empty_weight, trailer_make, trailer_type, trailer_license_number, trailer_state, trailer_empty_weight, total_gross_weight, height, width, combined_length, overhang_front, overhang_rear, overhang_left, overhang_right, load_description)
                    VALUES (@Id, @Date, @Status, @hauler_name, @hauler_email, @hauler_phone, @hauler_fax, @company_name, @company_address, @company_email, @company_contact, @company_phone, @company_fax, @insurance_provider, @insurance_agency_address, @insurance_policy_number, @vehicle_make, @vehicle_type, @vehicle_license_number, @vehicle_state, @vehicle_truck_serial_number, @vehicle_usdot_number, @vehicle_empty_weight, @trailer_make, @trailer_type, @trailer_license_number, @trailer_state, @trailer_empty_weight, @total_gross_weight, @height, @width, @combined_length, @overhang_front, @overhang_rear, @overhang_left, @overhang_right, @load_description)
                    ",
                    new
                    {
                        id = guid,
                        Date = DateTime.Now.ToString(),
                        Status = status,
                        hauler_name = permit.haulerinfoName,
                        hauler_email = permit.haulerinfoemail,
                        hauler_phone = permit.haulerinfophone,
                        hauler_fax = permit.haulerinfofax,
                        company_name = permit.companyinfoName,
                        company_address = permit.companyinfoAddress,
                        company_email = permit.companyinfoEmail,
                        company_contact = permit.companyinfoContact,
                        company_phone = permit.companyinfoPhone,
                        company_fax = permit.companyinfoFax,
                        insurance_provider = permit.insuranceinfoProvider,
                        insurance_agency_address = permit.insuranceinfoAgencyAddress,
                        insurance_policy_number = permit.insuranceinfoPolicyNumber,
                        vehicle_make = permit.vehicleinfoMake,
                        vehicle_type = permit.vehicleinfoType,
                        vehicle_license_number = permit.vehicleinfoLicense,
                        vehicle_state = permit.vehicleinfoState,
                        vehicle_truck_serial_number = permit.vehicleinfoSerialNumber,
                        vehicle_usdot_number = permit.vehicleinfoUSDOTNumber,
                        vehicle_empty_weight = permit.vehicleinfoEmptyWeight,
                        trailer_make = permit.trailerinfoMake,
                        trailer_type = permit.trailerinfoType,
                        trailer_license_number = permit.trailerinfoLicenseNumber,
                        trailer_state = permit.trailerinfoState,
                        trailer_empty_weight = permit.trailerinfoEmptyWeight,
                        total_gross_weight = permit.truckinfoGrossWeight,
                        height = permit.truckinfoHeight,
                        width = permit.truckinfoWidth,
                        combined_length = permit.truckinfoLength,
                        overhang_front = permit.truckinfoFrontOverhang,
                        overhang_rear = permit.truckinfoRearOverhang,
                        overhang_left = permit.truckinfoLeftOverhang,
                        overhang_right = permit.truckinfoRightOverhang,
                        load_description = permit.loadinfoDescription
                    });


                return guid;
            }
        }
	
        public int Count()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<int>(@"SELECT COUNT(*) FROM Permits").First();
            }
        }
    }
}
