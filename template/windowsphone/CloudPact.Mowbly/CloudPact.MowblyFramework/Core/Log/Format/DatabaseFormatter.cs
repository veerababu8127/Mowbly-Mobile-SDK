//-----------------------------------------------------------------------------------------
// <copyright file="DatabaseFormatter.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using System;
using System.Text;

namespace CloudPact.MowblyFramework.Core.Log.Format
{
    class DatabaseFormatter : ILogFormatter
    {
        const string DB_LOG_FORMAT = @"INSERT INTO logs(type,level,username,space,tag,message,timestamp) 
                                               VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";

        public String getContentType()
        {
            return String.Empty;
        }

        public String getHeader()
        {
            return null;
        }

        public String getFooter()
        {
            return null;
        }

        public string Format(LogEvent e)
        {
            string username = "";

            TimeSpan epochTime = new DateTime(e.TimeStamp) - new DateTime(1970, 1, 1);
            long epochTicks = (long)epochTime.TotalMilliseconds;
            string sql = string.Format(DB_LOG_FORMAT, new string[] 
            { 
                FormatString(e.Type),
                FormatString(e.Level.ToString()),
                FormatString(username),
                FormatString(""),
                FormatString(e.Tag),
                FormatString(e.Message),
                FormatString(Convert.ToString(epochTicks))
            });

            return sql;
        }

        string FormatString(string token)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(token.Replace("'", "''"));
            return sb.ToString();
        }
    }
}
