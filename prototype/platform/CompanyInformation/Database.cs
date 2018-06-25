using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;

namespace CompanyInformation
{
    public class Database : DataStore
    {
        public Database(HostConfigurationSection config)
            : base("CompanyInformation.sqlite", @"App_Data\Schema.sql", config)
        {
        }

        protected override void CreateDatabase()
        {
            base.CreateDatabase();

            // Populate the database with initial data if it was just created
            ImportTableFromCSV(@"App_Data\COMPANY_DATA.csv", "CompanyInformation", 100);
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

        private int CompanyCount()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM CompanyInformation");
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

        private void AssignUserToCompanies(IUserIdentity identity, int count)
        {
            // We know the company test data has primary keys from 1 .. N
            var numCompanies = CompanyCount();

            using (var conn = SimpleDbConnection())
            {
                foreach (var email in identity.EmailAddresses())
                {
                    // Draw the indicies 0 to n-1 and then add one to convert to a PK. Add in the email address, too
                    var values = DrawWithoutReplacement(numCompanies, count).Select(x => new { Email = email, CompanyId = x + 1 });

                    // Insert into the join table
                    conn.Execute(@"
                        INSERT INTO Users (user_email, company_id)
                        VALUES (@Email, @CompanyId)
                    ", values
                    );
                }
            }
        }

        public int Count()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<int>(@"SELECT COUNT(*) FROM CompanyInformation").First();
            }
        }

        public IEnumerable<dynamic> Query(int start, int length)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<dynamic>(@"
                    SELECT
                        company_name AS CompanyName,
                        email AS Email,
                        contact AS Contact,
                        phone AS Phone,
                        fax AS Fax,
                        cell AS Cell,
                        bill_to AS BillTo,
                        billing_address AS BillingAddress
                    FROM CompanyInformation
                    WHERE company_id NOT IN (
                        SELECT company_id
                        FROM CompanyInformation
                        ORDER BY company_name LIMIT @Start
                    )
                    ORDER BY company_name
                    LIMIT @Length
                    ",
                    new { Start = start, Length = length }
                    );
            }
        }

        public IEnumerable<dynamic> Users(int start, int length)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<dynamic>(@"
                    SELECT
                        user_email AS Email,
                        

                        contact AS Contact,
                        phone AS Phone,
                        fax AS Fax,
                        cell AS Cell,
                        bill_to AS BillTo,
                        billing_address AS BillingAddress
                    FROM Users U
                    JOIN CompanyInformation CI
                    ON U.company_id = CI.company_id
                    WHERE user_email NOT IN (
                        SELECT user_email
                        FROM Users
                        ORDER BY user_email LIMIT @Start
                    )
                    ORDER BY user_email
                    LIMIT @Length
                    ",
                    new { Start = start, Length = length }
                    );
            }
        }


        public IEnumerable<CompanyDatabaseRecord> FindCompanyInfoForUser(IUserIdentity identity)
        {
            // First, check if this user exists it the database.  If not, this is for prototyping, so
            // we randomly assign the user to have a few companies per email address
            if (!Exists(identity))
            {
                logger.Debug("User {0} does not exist. Adding to the company information", identity.UserName);
                AssignUserToCompanies(identity, 3);
            }

            // The email claim can be a string or an array of email addresses
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<CompanyDatabaseRecord>(@"
                    SELECT
                        CompanyInformation.company_id AS id,
                        company_name AS CompanyName,
                        email AS Email,
                        contact AS Contact,
                        phone AS Phone,
                        fax AS Fax,
                        cell AS Cell,
                        bill_to AS BillTo,
                        billing_address AS BillingAddress
                    FROM CompanyInformation
                    INNER JOIN Users
                    ON CompanyInformation.company_id = Users.company_id
                    WHERE user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    );
            }
        }
    }
    public class CompanyDatabaseRecord
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string BillingAddress { get; set; }
    }
}
