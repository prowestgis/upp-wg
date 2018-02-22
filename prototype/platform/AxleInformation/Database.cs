using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace AxleInformation
{
    public class Database : DataStore
    {
        public Database() : base("AxleInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\AXLE_DATA.csv", "AxleInformation", 100);
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

        private int AxleCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM AxleInformation");
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

        private void AssignUserAxleInfo(IUserIdentity identity, int count)
        {
            // We know the test data has primary keys from 1 .. N
            var numRecords = AxleCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numRecords, count).Select(x => new { Email = email, AxleId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, axle_id)
                        VALUES (@Email, @AxleId)
                    ", values
                    );
                }
            }
        }

        public IEnumerable<dynamic> FindAxlesInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few records per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the Axle information", identity.UserName);
                AssignUserAxleInfo(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<dynamic>(@"
                    SELECT
	                    axle_description as Description,
                        weight_per_axle as WeightPerAxle,
                        description_summary as DescriptionSummary,
                        axle_count as AxleCount,
                        group_count as GroupCount,
                        approx_axle_length as ApproxAxleLength,
                        axle_length as AxleLength,
                        max_axle_width as MaxAxleWidth,
                        axle_group_summary as AxleGroupSummary,
                        axles_per_group as AxlesPerGroup,
                        axle_group_tire_type as GroupTireType,
                        axle_group_width as GroupWidth,
                        axle_operating_weights as OperatingWeights,
                        axle_group_weight as GroupWeight,
                        axle_group_max_width as GroupMaxWidth,
                        axle_group_total_weight as GroupTotalWeight,
                        axle_group_distance as GroupDistance	
                    FROM AxleInformation
                    INNER JOIN Users
                    ON AxleInformation.axle_id = Users.axle_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }
    }
}
