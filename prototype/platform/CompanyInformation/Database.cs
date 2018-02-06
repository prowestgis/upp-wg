using System.Collections.Generic;
using UPP.Configuration;
using Dapper;

namespace CompanyInformation
{
    public class Database : DataStore
    {
        public Database() : base("CompanyInformation.sqlite", @"App_Data\Schema.sql")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // Populate the database with initial data
            ImportTableFromCSV(@"App_Data\COMPANY_DATA.csv", "CompanyInformation");
        }
    }
}
