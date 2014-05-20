//-----------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Utils;
using Community.CsharpSqlite.SQLiteClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Database : Feature
    {        
        Dictionary<string, DBConfig> dbConfigDict = new Dictionary<string, DBConfig>();

        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_DATABASE; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            string callbackId = message.CallbackId;
            try
            {
                // Variables
                SqliteConnection connection;
                DBConfig dbConfig;
                DbTransaction transaction;
                string connectionId, query, queryId;
                long dbSize;
                JObject o;

                switch (message.Method)
                {
                    case "openDatabase":

                        string dbName = message.Args[0] as string;
                        Int64 dblevel = (Int64)message.Args[1];
                        FileLevel level = (FileLevel)dblevel;
                        Int64 version = (Int64)message.Args[2];
                        float dbVersion = (float)version;
                        string dbPassword = message.Args[3] as string;
                        string dbPath = null;
                        if (dbName.Equals(Mowbly.GetProperty<string>(Constants.PROPERTY_LOGS_DB)))
                        {
                            dbPath = Mowbly.LogDatabaseFile;
                            if (!FileManager.FileExists(dbPath))
                            {
                                FileManager.CreateFile(dbPath);
                            }
                        }
                        else
                        {
                            dbPath = FileManager.GetAbsolutePath(new FilePath
                            {
                                Path = Path.Combine(Constants.DIR_DB, String.Concat(dbName, dbVersion.ToString())),
                                Level = level,
                                StorageType = StorageType.Internal
                            });
                        }

                        dbSize = FileManager.GetFileSize(dbPath);

                        // Create new connection
                        try
                        {
                            connection = new SqliteConnection();
                            connection.ConnectionString = DBUtils.GetConnectionString(dbPath, dbPassword);
                            connection.Open();
                            connectionId = Guid.NewGuid().ToString();
                            dbConfigDict.Add(connectionId, new DBConfig
                            {
                                DBName = dbName,
                                DBVersion = dbVersion,
                                DBPath = dbPath,
                                DBPassword = dbPassword,
                                Connection = connection
                            });

                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = connectionId });
                        }
                        catch (SqliteException se)
                        {
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_OPEN_ERROR), se.Message);
                            Logger.Error(error);
                            JToken opt = message.Args[0] as JToken;
                            string Id = opt["queryId"].ToObject<string>();
                            JObject obj = new JObject();
                            obj.Add("queryId", Id);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = MethodResult.FAILURE_CODE,
                                    Result = obj,
                                    Error = new MethodError
                                    {
                                        Message = error
                                    }
                                });
                        }

                        break;

                    case "executeQuery":

                        // Get connection id
                        JToken options = message.Args[0] as JToken;
                        connectionId = options["id"].ToObject<string>();
                        dbConfig = dbConfigDict[connectionId];

                        // Read args
                        queryId = options["queryId"].ToObject<string>();
                        query = options["sql"].ToObject<string>().Trim();
                        List<object> queryParams = options["params"].ToObject<List<object>>();

                        // Execute query
                        try
                        {
                            JArray data = null;
                            int rowsAffected = 0;
                            connection = dbConfig.Connection;

                            // Throw exception is connection is null
                            if (connection == null)
                            {
                                throw new ArgumentException(Mowbly.GetString(Constants.STRING_DATABASE_NO_CONNECTION_OPEN_ERROR));
                            }

                            // Execute query
                            if (query.ToLower().StartsWith("select"))
                            {
                                data = ProcessSelectQuery(ref connection, query, queryParams);
                            }
                            else
                            {
                                rowsAffected = ProcessNonQuery(ref connection, query, queryParams);
                            }

                            // Create result
                            o = new JObject();
                            o.Add("queryId", queryId);

                            JObject d = new JObject();
                            d.Add("rowsAffected", rowsAffected);
                            d.Add("insertId", connection.LastInsertRowId);
                            d.Add("rows", data);
                            o.Add("data", d);

                            // Notify JS
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = o });
                        }
                        catch (SqliteException se)
                        {
                            // Error
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_QUERY_ERROR), se.Message);
                            Logger.Error(error);

                            // Create result
                            o = new JObject();
                            o.Add("queryId", queryId);

                            // Notify Js
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = o,
                                Error = new MethodError
                                {
                                    Message = error
                                }
                            });
                        }

                        break;

                    case "beginTransaction":

                        connectionId = message.Args[0] as string;
                        dbConfig = dbConfigDict[connectionId];

                        // Begin transaction
                        try
                        {
                            connection = dbConfig.Connection;
                            // Throw exception is connection is null
                            if (connection == null)
                            {
                                throw new ArgumentException(Mowbly.GetString(Constants.STRING_DATABASE_NO_CONNECTION_OPEN_ERROR));
                            }

                            transaction = connection.BeginTransaction();
                            dbConfig.Transaction = transaction;

                            // Notify JS
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (SqliteException se)
                        {
                            // Error; Notify JS
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_TRANSACTION_ERROR), se.Message);
                            Logger.Error(error);
                            JToken opt = message.Args[0] as JToken;
                            string Id = opt["queryId"].ToObject<string>();
                            JObject obj = new JObject();
                            obj.Add("queryId", Id);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = obj,
                                Error = new MethodError
                                {
                                    Message = error
                                }
                            });
                        }

                        break;

                    case "commit":

                        connectionId = message.Args[0] as string;
                        dbConfig = dbConfigDict[connectionId];

                        // Commit transaction
                        try
                        {
                            transaction = dbConfig.Transaction;
                            // Throw exception is transaction is null
                            if (transaction == null)
                            {
                                throw new ArgumentException(Mowbly.GetString(Constants.STRING_DATABASE_NO_TRANSACTION_ACTIVE_ERROR));
                            }
                            transaction.Commit();

                            queryId = message.Args[1] as string;
                            o = new JObject();
                            o["queryId"] = queryId;
                            o["data"] = true;

                            // Notify JS
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = o });
                        }
                        catch (SqliteException se)
                        {
                            // Error; Notify JS
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_TRANSACTION_ERROR), se.Message);
                            Logger.Error(error);
                            JToken opt = message.Args[0] as JToken;
                            string Id = opt["queryId"].ToObject<string>();
                            JObject obj = new JObject();
                            obj.Add("queryId", Id);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = obj,
                                Error = new MethodError
                                {
                                    Message = error
                                }
                            });
                        }

                        break;

                    case "rollback":

                        connectionId = message.Args[0] as string;
                        dbConfig = dbConfigDict[connectionId];

                        // Commit transaction
                        try
                        {
                            transaction = dbConfig.Transaction;
                            transaction.Rollback();

                            queryId = message.Args[1] as string;
                            o = new JObject();
                            o["queryId"] = queryId;
                            o["data"] = true;

                            // Notify JS
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = o });
                        }
                        catch (SqliteException se)
                        {
                            // Error; Notify JS
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_TRANSACTION_ERROR), se.Message);
                            Logger.Error(error);
                            JToken opt = message.Args[0] as JToken;
                            string Id = opt["queryId"].ToObject<string>();
                            JObject obj = new JObject();
                            obj.Add("queryId", Id);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = obj,
                                Error = new MethodError
                                {
                                    Message = error
                                }
                            });
                        }

                        break;

                    default:
                        Logger.Error("Feature " + Name + " does not support method " + message.Method);
                        break;
                }
            }
            catch (Exception e)
            {
                // Error; Notify JS
                string error = String.Concat(Mowbly.GetString(Constants.STRING_DATABASE_OPERATION_ERROR), e.Message);
                Logger.Error(error);
                JToken opt = message.Args[0] as JToken;
                string Id = opt["queryId"].ToObject<string>();
                JObject obj = new JObject();
                obj.Add("queryId", Id);
                InvokeCallbackJavascript(callbackId, new MethodResult
                {
                    Code = MethodResult.FAILURE_CODE,
                    Result = obj,
                    Error = new MethodError
                    {
                        Message = error
                    }
                });
            }
            await Task.FromResult(0);
        }

        #endregion

        #region Database

        // Execute sql query
        int ProcessNonQuery(ref SqliteConnection conn, string sql, List<object> sqlParams)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            if (sqlParams != null && sqlParams.Count > 0)
            {
                foreach (object value in sqlParams)
                {
                    IDataParameter param = cmd.CreateParameter();
                    param.Value = value;
                    cmd.Parameters.Add(param);
                }
            }

            int rowsAffected = cmd.ExecuteNonQuery();
            //TODO: Obtain command last insert row id.
            return rowsAffected;
        }

        // Execute select sql query
        JArray ProcessSelectQuery(ref SqliteConnection conn, string sql, List<object> sqlParams)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (sqlParams != null && sqlParams.Count > 0)
            {
                foreach (object value in sqlParams)
                {
                    IDataParameter param = cmd.CreateParameter();
                    param.Value = value;
                    cmd.Parameters.Add(param);
                }
            }
            JArray array = null;
            using (IDataReader rd = cmd.ExecuteReader())
            {
                Logger.Debug("Records Affected: " + rd.RecordsAffected);
                int count = rd.FieldCount;
                array = new JArray();

                while (rd.Read())
                {
                    string[] names = new string[count];
                    JObject r = new JObject();
                    for (int j = 0; j < count; ++j)
                    {
                        names[j] = rd.GetName(j);
                        object value = rd.GetValue(j);
                        r.Add(names[j], (value == null) ? null : JToken.FromObject(value));
                    }
                    array.Add(r);
                }
            }
            return array;
        }

        #endregion

        #region DBConfig

        public struct DBConfig
        {
            public string DBName { get; set; }

            public float DBVersion { get; set; }

            public string DBPath { get; set; }

            public string DBPassword { get; set; }

            public SqliteConnection Connection { get; set; }

            public DbTransaction Transaction { get; set; }
        }

        #endregion
    }
}
