using System;
using System.Collections.Generic;
using System.Linq;
using UPP.Configuration;
using Dapper;
using UPP.Common;
using Nancy.Security;
using UPP.Protocols;

namespace PermitIssuer
{
    public static class PermitStatus
    {
        public const int NEW = 0;
        public const int APPROVED = 1;
        public const int REJECTED = 2;
        public const int UNDER_REVIEW = 3;
        public static string Text(int status) {
            switch (status)
            {
                case NEW: return "NEW";
                case APPROVED: return "APPROVED";
                case REJECTED: return "REJECTED";
                case UNDER_REVIEW: return "UNDER REVIEW";
                default:
                    return String.Empty;
            }
        }
    }

    public class Database : DataStore
    {
        public Database(HostConfigurationSection config)
            : base("Permits.sqlite", @"App_Data\Schema.sql", config)
        {
        }

        private int? FindUserId(IUserIdentity identity)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<int?>(@"
                    SELECT U.user_id                        
                    FROM Emails E
                    INNER JOIN Users U
                    ON E.user_id = U.user_id
                    WHERE U.is_active = 1 AND E.user_email IN @Emails
                    ",
                    new { Emails = identity.EmailAddresses() }
                    ).FirstOrDefault();
            }
        }

        private int CreateEmptyUser()
        {
            using (var conn = SimpleDbConnection())
            {
                var sql = @"INSERT INTO Users (is_active) VALUES (1); SELECT last_insert_rowid()";
                return conn.Query<int>(sql).First();
            }
        }

        private void AddEmailAddressesToUser(int userId, List<string> emails)
        {
            if (!emails.Any())
            {
                return;
            }

            using (var conn = SimpleDbConnection())
            {
                conn.Execute(@"
                    INSERT INTO Emails VALUES (@UserId, @UserEmail)
                    ", emails.Select(x => new { UserId = userId, UserEmail = x }));
            }
        }

        private List<string> GetEmailAddressesForUser(int userId)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<string>("SELECT user_email FROM Emails WHERE user_id = @UserId", new { UserId = userId }).ToList();
            }
        }

        public List<dynamic> FindPermitsForUser(IUserIdentity identity)
        {
            using (var conn = SimpleDbConnection())
            {
                var emailAddresses = identity.EmailAddresses();
                return conn.Query<dynamic>(@"
                    SELECT A.application_id AS Id, A.application_status AS Status, A.application_data AS Data
                    FROM Applications A
                    LEFT JOIN UserApplications UA ON A.application_id = UA.application_id
                    LEFT JOIN Emails E ON UA.user_id = E.user_id
                    LEFT JOIN Users U ON UA.user_id = U.user_id
                    WHERE U.is_active = 1 AND E.user_email IN (@Addresses) 
                ", new { Addresses = emailAddresses })
                .ToList();
            }
        }

        public dynamic CreatePermitForUser(IUserIdentity identity, string blob)
        {
            // First, make sure the user exists. If not, create a new user record
            var userId = FindUserId(identity);
            if (!userId.HasValue)
            {
                userId = CreateEmptyUser();
            }

            // Now, check to make sure that all of the user's email addresses are associated with them
            var emails = identity.EmailAddresses();
            var dbEmails = GetEmailAddressesForUser(userId.Value);

            // If there are any emails that are missing, add them
            AddEmailAddressesToUser(userId.Value, emails.Where(e => !dbEmails.Contains(e)).ToList());

            // Finally we can create the actual permit application
            using (var conn = SimpleDbConnection())
            {
                var appId = conn.Query<int>(@"
                    INSERT INTO Applications (application_data, application_status) VALUES (@Data, @Status); SELECT last_insert_rowid()
                    ", new { Data = blob, Status = PermitStatus.NEW }).First();

                conn.Execute(@"INSERT INTO UserApplications (user_id, application_id) VALUES (@UserId, @AppId)", new { UserId = userId.Value, AppId = appId });
            }

            return null;
        }
        public dynamic UpdateApplication( string blob, int id, int status)
        {
            using (var conn = SimpleDbConnection())
            {
                conn.Execute(@"
                    UPDATE Applications SET application_status = @Status WHERE Application_id = @Id
                    ", new {  Status = status, Id =  id});

            }

            return null;
        }

        public List<PermitApplication> AllPermits()
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<PermitApplication>(@"
                    SELECT application_id as Id , application_data as Data, application_status as Status
                    FROM Applications
                    "
                 ).ToList();
            }
        }
        public PermitApplication ApplicationById(int id)
        {
            using (var conn = SimpleDbConnection())
            {
                return conn.Query<PermitApplication>(@"
                    SELECT application_id as Id , application_data as Data, application_status as Status
                    FROM Applications
                    WHERE application_id = @id
                    ", new { id = id}
                ).First();
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

    public sealed class PermitApplication
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }

        public PermitApplicationRecord Application { get { return Newtonsoft.Json.JsonConvert.DeserializeObject<PermitApplicationRecord>(Data); }  }
        public PermitDataRecord PermitData { get; set; }
    }
}
