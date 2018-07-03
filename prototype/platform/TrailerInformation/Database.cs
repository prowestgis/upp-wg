using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace TrailerInformation
{
    public class Database : DataStore
    {
        public Database() : base("TrailerInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\TRAILER_DATA.csv", "TrailerInformation", 100);
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

        private int TrailerCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM TrailerInformation");
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

        private void AssignUserTrailerInfo(IUserIdentity identity, int count)
        {
            // We know the test data has primary keys from 1 .. N
            var numRecords = TrailerCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numRecords, count).Select(x => new { Email = email, TrailerId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, trailer_id)
                        VALUES (@Email, @TrailerId)
                    ", values
                    );
                }
            }
        }

        public IEnumerable<TrailerInformationRecord> FindTrailersInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few records per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the Trailer information", identity.UserName);
                AssignUserTrailerInfo(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<TrailerInformationRecord>(@"
                    SELECT
                        TrailerInformation.trailer_id as Id,
	                    trailer_description as Description,
	                    make as Make,
	                    model as Model,
	                    trailer_type as TrailerType,
                        serial_number as SerialNumber,
	                    license_number as License,
	                    trailer_state as State,
	                    empty_weight as EmptyWeight,
	                    registered_weight as RegisteredWeight,
                        regulation_weight as RegulationWeight
                    FROM TrailerInformation
                    INNER JOIN Users
                    ON TrailerInformation.trailer_id = Users.trailer_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }

        public sealed class TrailerInformationRecord : IResourceObject
        {
            public string Description { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string TrailerType { get; set; }
            public string SerialNumber { get; set; }
            public string License { get; set; }
            public string State { get; set; }
            public decimal EmptyWeight { get; set; }
            public decimal RegisteredWeight { get; set; }
            public decimal RegulationWeight { get; set; }

            public object Id { get; set; }
            public string Type { get { return "trailer-information"; } }
        }
    }
}
