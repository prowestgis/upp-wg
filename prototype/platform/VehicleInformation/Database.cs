using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace VehicleInformation
{
    public class Database : DataStore
    {
        public Database() : base("VehicleInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\Vehicle_DATA.csv", "VehicleInformation", 100);
        }

        private bool Exists(IUserIdentity identity)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<dynamic>(@"
                    SELECT
                        1
                    FROM Users
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    ).Any();
            }
        }

        private int VehicleCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM VehicleInformation");
            }
        }

        // Fisher-Yates shuffle to pick the first k of n items
        private IEnumerable<int> DrawWithoutReplacement(int n, int k)
        {
            if (n < 0 || k < 0 || n < k) throw new ArgumentException("Invalid arguments");

            var rng = new Random();
            var arr = Enumerable.Range(0, n).ToArray();
            
            for (int i = 0; i < k; i++)
            {
                // Draw j such that i <= j < n
                int j = rng.Next(i, n);

                // Swap
                var tmp = arr[i];
                arr[i] = arr[j];
                arr[j] = tmp;

                // Swap and return the new index
                yield return arr[i];
            }
        }

        private void AssignUserVehicleInfo(IUserIdentity identity, int count)
        {
            // We know the test data has primary keys from 1 .. N
            var numRecords = VehicleCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numRecords, count).Select(x => new { Email = email, VehicleId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, vehicle_id)
                        VALUES (@Email, @VehicleId)
                    ", values
                    );
                }
            }
        }

        public IEnumerable<VehicleInformationRecord> FindVehiclesInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few records per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the vehicle information", identity.UserName);
                AssignUserVehicleInfo(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<VehicleInformationRecord>(@"
                    SELECT
	                    vehicle_year as Year,
	                    make as Make,
	                    model as Model,
	                    vehicle_type as Type,
	                    license_number as License,
	                    vehicle_state as State,
	                    serial_number as SerialNumber,
	                    usdot_number as USDOTNumber,
	                    empty_weight as EmptyWeight,
	                    registered_weight as RegisteredWeight
                    FROM VehicleInformation
                    INNER JOIN Users
                    ON VehicleInformation.vehicle_id = Users.vehicle_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }

        public sealed class VehicleInformationRecord : IResourceObject
        {
            public string Year { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string VehicleType { get; set; }
            public string License { get; set; }
            public string State { get; set; }
            public string SerialNumber { get; set; }
            public string USDOTNumber { get; set; }

            public decimal RegisteredWeight { get; set; }
            public decimal RegulationWeight { get; set; }

            public object Id { get; set; }
            public string Type { get { return "vehicle-information"; } }
        }
    }
}
