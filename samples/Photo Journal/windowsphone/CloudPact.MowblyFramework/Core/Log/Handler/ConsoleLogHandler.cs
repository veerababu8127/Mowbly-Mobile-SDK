//-----------------------------------------------------------------------------------------
// <copyright file="ConsoleLogHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Format;
using System.Diagnostics;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    class ConsoleLogHandler : AbstractLogHandler
    {
        public ConsoleLogHandler()
            : base()
        {
            this.Name = "__console_log_handler";
            this.Formatter = new ConsoleFormatter();
        }

        public override void Handle(LogEvent e)
        {
            Debug.WriteLine(this.Formatter.Format(e));
        }
    }
}