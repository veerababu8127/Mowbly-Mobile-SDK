//-----------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class DateTimeUtils
    {
        /// <summary>
        /// Returns the current epoch timestamp
        /// </summary>
        /// <returns>Epoch timestamp</returns>
        internal static long GetEpochTimestamp()
        {
            DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timestamp = DateTime.UtcNow - Jan1970;
            return (long)timestamp.TotalMilliseconds;
        }
    }
    
    class DBUtils
    {
        /// <summary>
        /// Returns the SqliteConnection string for the specified database parameters
        /// </summary>
        /// <param name="path">File path of the database</param>
        /// <param name="password">Password of the database</param>
        /// <returns>SqliteConnection connection string</returns>
        internal static string GetConnectionString(string path, string password)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0}", path);
            if (!string.IsNullOrEmpty(password))
            {
                sb.AppendFormat("Password={0}", password);
            }
            return sb.ToString();
        }
    }

    class GCUtils
    {
        /// <summary>
        /// Cleans up memory of the closed.Memory has to be reclaimed
        /// forcedly .Done asynchronously so the caller method is returned
        /// to make the current object orphan.
        /// </summary>
        internal static void DoGC()
        {
            try
            {
                Thread t = new Thread(() => LazyGcCollect(4000));
                t.Start();
            }
            catch { }
        }

        /// <summary>
        /// Releases memory in a seperate thread.
        /// </summary>
        static void LazyGcCollect(int timeout)
        {
            try
            {
                Thread.Sleep(timeout);
                Logger.Debug("Doing Gc...");
                System.GC.Collect();
            }
            catch { }
        }
    }

    class ObjectSerializer
    {
        /// <summary>
        /// Serializes the specified DataContract object and returns as string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="T">Type of the object</param>
        /// <returns>Serialized string of the object</returns>
        internal static string Serialize(object obj, Type T)
        {
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    DataContractSerializer serializer =
                        new DataContractSerializer(T);
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Indent = true,
                        IndentChars = "\t"
                    };
                    serializer.WriteObject(xw, obj);
                }
                return sw.ToString();
            }
        }
    }
}