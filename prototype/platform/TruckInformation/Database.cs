using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace TruckInformation
{
    public class Database : DataStore
    {
        public Database() : base("TruckInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\TRUCK_DATA.csv", "TruckInformation", 100);
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

        private int TruckCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM TruckInformation");
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

        private void AssignUserTruckInfo(IUserIdentity identity, int count)
        {
            // We know the test data has primary keys from 1 .. N
            var numRecords = TruckCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numRecords, count).Select(x => new { Email = email, TruckId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, truck_id)
                        VALUES (@Email, @TruckId)
                    ", values
                    );
                }
            }
        }

        public IEnumerable<TruckInformationRecord> FindTrucksInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few records per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the Truck information", identity.UserName);
                AssignUserTruckInfo(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<TruckInformationRecord>(@"
                    SELECT
                        TruckInformation.truck_id as Id,
	                    gross_weight as GrossWeight,
	                    empty_weight as EmptyWeight,
	                    registered_weight as RegisteredWeight,
                        regulation_weight as RegulationWeight,
	                    height as Height,
	                    width as Width,
	                    truck_length as Length,
	                    front_overhang as FrontOverhang,
	                    rear_overhang as RearOverhang,
	                    left_overhang as LeftOverhang,
	                    right_overhang as RightOverhang,
	                    diagram as Diagram
                    FROM TruckInformation
                    INNER JOIN Users
                    ON TruckInformation.truck_id = Users.truck_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }

        public sealed class TruckInformationRecord : IResourceObject
        {
            public string Diagram { get; set; }

            public decimal FrontOverhang { get; set; }
            public decimal RearOverhang { get; set; }
            public decimal LeftOverhang { get; set; }
            public decimal RightOverhang { get; set; }

            public decimal Height { get; set; }
            public decimal Width { get; set; }
            public decimal Length { get; set; }

            public decimal EmptyWeight { get; set; }
            public decimal GrossWeight { get; set; }
            public decimal RegisteredWeight { get; set; }
            public decimal RegulationWeight { get; set; }

            public object Id { get; set; }
            public string Type { get { return "truck-information"; } }
        }
    }
}
