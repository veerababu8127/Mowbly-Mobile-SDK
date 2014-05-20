//-----------------------------------------------------------------------------------------
// <copyright file="SimpleFormatter.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Utils;
using System;
using System.Text;

namespace CloudPact.MowblyFramework.Core.Log.Format
{
    class SimpleFormatter : ILogFormatter
    {
        public String getContentType()
        {
            return "text/plain";
        }

        public String getHeader()
        {
            return null;
        }

        public String getFooter()
        {
            return null;
        }

        public virtual string Format(LogEvent e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0:yyyy-MM-dd hh:mm:ss.fff}", DateTime.Now));
            sb.Append(" ");
            sb.Append(e.Type);
            sb.Append(" ");
            sb.Append(String.Format("[{0}]", e.Level.ToString().ToUpper()));
            sb.Append(" ");
            sb.Append(String.Format("[{0}]", e.Username));
            sb.Append(" ");
            sb.Append(String.Format("[{0}]",e.Space));
            sb.Append(" ");
            sb.Append(e.Tag);
            sb.Append(" ");
            sb.Append("-");
            sb.Append(" ");
            sb.Append(e.Message.Replace(Environment.NewLine, @" \n "));
            sb.Append(" ");

            return sb.ToString();
        }
    }
}