//-----------------------------------------------------------------------------------------
// <copyright file="DatabaseLogHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Format;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Utils;
using Community.CsharpSqlite.SQLiteClient;
using System;
using System.Data.Common;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    class DatabaseLogHandler : AbstractLogHandler
    {
        string logsDbName;

        float logsDbVersion;

        string logsDbPath;

        string logsDbPassword;

        SqliteConnection connection;
        
        const string DB_CREATE_SQL_FORMAT = @"CREATE TABLE IF NOT EXISTS logs (
                                                _id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                type VARCHAR(255),
                                                username VARCHAR(255),
                                                space VARCHAR(255),
                                                level VARCHAR(50),
                                                tag TEXT,
                                                message LONGTEXT,
                                                timestamp LONGTEXT)";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbName">Name of the database</param>
        /// <param name="dbVersion">Version of the database</param>
        /// <param name="dbPath">Path to file of the database</param>
        /// <param name="dbPassword">Password of the database</param>
        public DatabaseLogHandler(string dbName, float dbVersion, string dbPath, string dbPassword)
            : base()
        {
            this.Name = "__database_log_handler";
            this.Formatter = new DatabaseFormatter();

            this.logsDbName = dbName;
            this.logsDbVersion = dbVersion;
            this.logsDbPath = dbPath;
            this.logsDbPassword = dbPassword;

            Setup();
        }

        #region ILogHandler
        
        // Setup logs database
        public override void Setup()
        {
            // Create the connection
            if (connection == null)
            {
                connection = new SqliteConnection();
            }

            // Get the connection string
            string connectionString = DBUtils.GetConnectionString(logsDbPath, logsDbPassword);

            // Create the DB file if not exists
            if (!FileManager.FileExists(logsDbPath))
            {
                FileManager.CreateFile(logsDbPath);
            }

            // Create db
            connection.ConnectionString = connectionString;
            connection.Open();
            ExecuteSql(DB_CREATE_SQL_FORMAT);
        }

        public override void Start()
        {
            Setup();
        }

        public override void Handle(LogEvent e)
        {
            if (connection != null)
            {
                ExecuteSql(Formatter.Format(e));
            }
        }

        public override void Shutdown()
        {
            // Dispose connection
            try
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
            catch { }
        }

        public override void Reset()
        {
            try
            {
                // Shutdown
                Shutdown();

                // Delete the log file
                FileManager.DeleteFile(this.logsDbPath);
                
                // Start
                Start();
            }
            catch (Exception e)
            {
                Logger.Warn("Error resetting FileLogHandler. Reason " + e.Message);
            }
        }

        #endregion

        #region Private methods

        // Executes database operation
        private void ExecuteSql(string sql)
        {
            lock (connection)
            {
                try
                {
                    DbTransaction transaction = connection.BeginTransaction();
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Logger.Error("Logs: Error in database operation. Reason - " + e.Message);
                }
            }
        }

        #endregion

        #region IDisposable

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage", 
            "CA2213:DisposableFieldsShouldBeDisposed", 
            MessageId = "connection")]
        public override void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    // Shutdown handler
                    Shutdown();

                    isDisposed = true;
                }
            }
            catch {}
        }

        #endregion
    }
}