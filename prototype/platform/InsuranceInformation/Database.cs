using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace InsuranceInformation
{
    public class Database : DataStore
    {
        public Database() : base("InsuranceInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\INSURANCE_DATA.csv", "InsuranceInformation", 100);
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

        private int InsurerCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM InsuranceInformation");
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

        private void AssignUserInsuranceInfo(IUserIdentity identity, int count)
        {
            // We know the test data has primary keys from 1 .. N
            var numInsurers = InsurerCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numInsurers, count).Select(x => new { Email = email, InsuranceId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, insurance_id)
                        VALUES (@Email, @InsuranceId)
                    ", values
                    );
                }
            }
        }

        public IEnumerable<InsuranceInformationRecord> FindInsuranceInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few records per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the insurance information", identity.UserName);
                AssignUserInsuranceInfo(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<InsuranceInformationRecord>(@"
                    SELECT
                        InsuranceInformation.insurance_id as Id,
                        provider_name AS ProviderName,
                        agency_address AS AgencyAddress,
                        policy_number AS PolicyNumber,
                        insured_amount AS InsuredAmount
                    FROM InsuranceInformation
                    INNER JOIN Users
                    ON InsuranceInformation.insurance_id = Users.insurance_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }

        public sealed class InsuranceInformationRecord : IResourceObject
        {
            public string ProviderName { get; set; }
            public string AgencyAddress { get; set; }
            public string PolicyNumber { get; set; }
            public decimal InsuredAmount { get; set; }

            public object Id { get; set; }
            public string Type { get { return "insurance-information"; } }
        }
    }
}
