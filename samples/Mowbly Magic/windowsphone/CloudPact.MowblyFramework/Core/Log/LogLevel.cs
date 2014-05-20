//-----------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

namespace CloudPact.MowblyFramework.Core.Log
{
    class LogLevel : LogPriority
    {
        public static LogLevel DEBUG_LEVEL = new LogLevel((int)Log_Priority.Debug, "DEBUG");

        public static LogLevel INFO_LEVEL = new LogLevel((int)Log_Priority.Info, "INFO");

        public static LogLevel WARN_LEVEL = new LogLevel((int)Log_Priority.Warn, "WARN");

        public static LogLevel ERROR_LEVEL = new LogLevel((int)Log_Priority.Error, "ERROR");

        public static LogLevel FATAL_LEVEL = new LogLevel((int)Log_Priority.Fatal, "FATAL");

        public static LogLevel THRESHOLD_LEVEL = new LogLevel((int)Log_Priority.Debug, "DEBUG");

        private LogLevel(int priority, string priorityStr) :
            base(priority, priorityStr) { }

        public static LogLevel from(string level_number)
        {
            LogLevel LogLevelForNumber = null;
            switch (level_number)
            {
                case "10000":
                    LogLevelForNumber = LogLevel.DEBUG_LEVEL;
                    break;
                case "20000":
                    LogLevelForNumber = LogLevel.INFO_LEVEL;
                    break;
                case "30000":
                    LogLevelForNumber = LogLevel.WARN_LEVEL;
                    break;
                case "40000":
                    LogLevelForNumber = LogLevel.ERROR_LEVEL;
                    break;
                case "50000":
                    LogLevelForNumber = LogLevel.FATAL_LEVEL;
                    break;
            }
            return LogLevelForNumber;
        }
    }
}