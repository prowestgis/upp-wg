﻿using NLog;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace UPP.Configuration
{
    /// <summary>
    /// Abstract class to take care of initializing and providing uniform access to simple
    /// SQLite data stores for the microservice providers.
    /// </summary>
    public abstract class DataStore
    {
        private const string databaseFileExtension = ".sqlite";
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly string dbFile;
        protected readonly string schemaFile;

        protected DataStoreOptions options;
        
        public sealed class DataStoreOptions
        {
            public DataStoreOptions(string databaseFile, string schemaFile)
            {
                DeleteDatabaseOnStartup = true;
                CreateDatabaseOnStartup = true;
                DatabaseFile = databaseFile;
                SchemaFile = schemaFile;
            }

            public DataStoreOptions(string databaseFile, string schemaFile, HostConfigurationSection config)
            {
                DeleteDatabaseOnStartup = config.Keyword<bool>(Keys.DATABASE__DELETE_ON_STARTUP);
                CreateDatabaseOnStartup = config.Keyword<bool>(Keys.DATABASE__CREATE_ON_STARTUP);
                DatabaseFile = config.Keyword(Keys.DATABASE__FILE, () => databaseFile);
                SchemaFile = config.Keyword(Keys.DATABASE__SCHEMA, () => schemaFile);
            }

            public DataStoreOptions(HostConfigurationSection config)
            {
                DeleteDatabaseOnStartup = config.Keyword<bool>(Keys.DATABASE__DELETE_ON_STARTUP);
                CreateDatabaseOnStartup = config.Keyword<bool>(Keys.DATABASE__CREATE_ON_STARTUP);
                DatabaseFile = config.Keyword(Keys.DATABASE__FILE);
                SchemaFile = config.Keyword(Keys.DATABASE__SCHEMA);
            }

            public DataStoreOptions()
            {
                DeleteDatabaseOnStartup = true;
                CreateDatabaseOnStartup = true;
                DatabaseFile = null;
                SchemaFile = null;
            }

            public string DatabaseFile { get; set; }
            public string SchemaFile { get; set; }
            public bool DeleteDatabaseOnStartup { get; set; }
            public bool CreateDatabaseOnStartup { get; set; }
        }

        public static string ResolvePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(baseDirectory, path);
        }

        public DataStore(string databaseFile, string schemaFile, HostConfigurationSection config)
            : this(new DataStoreOptions(databaseFile, schemaFile, config))
        {            
        }

        public DataStore(string databaseFile, string schemaFile)
            : this(new DataStoreOptions(databaseFile, schemaFile))
        {
        }

        public DataStore(DataStoreOptions options)
        {
            var databaseFile = options.DatabaseFile;
            var schemaFile = options.SchemaFile;

            if (String.IsNullOrEmpty(databaseFile))
            {
                throw new ArgumentException("Database name is not specified");
            }

            if (String.IsNullOrEmpty(schemaFile))
            {
                throw new ArgumentException("Schema file is not specified");
            }

            if (options == null)
            {
                options = new DataStoreOptions();
            }

            // Put in the current directory if it's not an absolute path
            schemaFile = ResolvePath(schemaFile);

            if (!File.Exists(schemaFile))
            {
                throw new ArgumentException(String.Format("Schema file does not exist at {0}", schemaFile), "schemaFile");
            }

            // Automatically append a suffix if there isn't one
            if (!Path.HasExtension(databaseFile))
            {
                databaseFile += databaseFileExtension;
            }

            // Put in the current directory if it's not an absolute path
            databaseFile = ResolvePath(databaseFile);

            this.dbFile = databaseFile;
            this.schemaFile = schemaFile;
            this.options = options;
        }

        protected DbConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + dbFile);
        }

        protected virtual void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var command = cnn.CreateCommand();
                command.CommandText = File.ReadAllText(schemaFile);
                command.ExecuteNonQuery();
            }
        }

        public virtual void Initialize()
        {
            if (File.Exists(dbFile) && options.DeleteDatabaseOnStartup)
            {
                logger.Info("Deleting database at {0}", dbFile);
                File.Delete(dbFile);
            }

            if (!File.Exists(dbFile) && options.CreateDatabaseOnStartup)
            {
                logger.Info("Database file does not exist. Creating database at {0}", dbFile);
                CreateDatabase();
            }            
        }

        /// <summary>
        /// Used to populate test data: https://www.mockaroo.com/
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="tableName"></param>
        protected void ImportTableFromCSV(string csvFile, string tableName, int maximum = 1000)
        {
            csvFile = ResolvePath(csvFile);

            if (!File.Exists(csvFile))
            {
                logger.Warn("CSV file '{0}' does not exist", csvFile);
                return;
            }

            logger.Info("Importing data from {0} into database table {1}", csvFile, tableName);
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvHelper.CsvReader(reader))
            using (var conn = SimpleDbConnection())
            {
                var records = new List<dynamic>();

                // Assume the CSV header matches the database fields
                csv.Read();
                csv.ReadHeader();
                for (int i = 0; i < maximum && csv.Read(); i++)
                {
                    records.Add(csv.GetRecord<dynamic>());
                }

                // Extract the header names
                var tableFields = new List<string>();
                var objectFields = new List<string>();
                foreach (var header in csv.Context.HeaderRecord)
                {
                    tableFields.Add(header);
                    objectFields.Add("@" + header);
                }

                // Insert the records
                var sql = String.Format(@"
                    INSERT INTO {0}({1})
                    VALUES ({2})
                ", tableName, String.Join(", ", tableFields), String.Join(", ", objectFields));

                conn.Execute(sql, records);
            }
        }
    }
}
