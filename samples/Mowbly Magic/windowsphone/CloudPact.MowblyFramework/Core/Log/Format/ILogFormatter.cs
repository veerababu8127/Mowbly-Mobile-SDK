//-----------------------------------------------------------------------------------------
// <copyright file="ILogFormatter.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;

namespace CloudPact.MowblyFramework.Core.Log.Format
{
    interface ILogFormatter
    {
        /// <summary>
        /// Content type of the log
        /// </summary>
        /// <returns>Content type of the log</returns>
        String getContentType();

        /// <summary>
        /// Header content of the log
        /// </summary>
        /// <returns>Header content of the log</returns>
        String getHeader();

        /// <summary>
        /// Footer content of the log
        /// </summary>
        /// <returns>Footer content of the log</returns>
        String getFooter();

        /// <summary>
        /// Formats the log message and returns the output string message.
        /// </summary>
        /// <param name="e">
        /// <see cref="CloudPact.MowblyFramework.Core.Log.LogEvent">LogEvent</see> object
        /// </param>
        /// <returns>Formatted log string</returns>
        String Format(LogEvent e);
    }
}
