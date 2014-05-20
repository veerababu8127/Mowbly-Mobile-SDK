//-----------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using System;

namespace CloudPact.MowblyFramework.Core.Log
{
    class Logger : AbstractLogger
    {
        #region Static System Logger Helpers

        internal static Logger systemLogger = LoggerManager.Instance.GetLogger(LoggerManager.TYPE_SYSTEM_LOGGER);

        internal static void Debug(string message)
        {
            systemLogger.Debug(LoggerManager.TYPE_SYSTEM_LOGGER, message);
        }

        internal static void Info(string message)
        {
            systemLogger.Info(LoggerManager.TYPE_SYSTEM_LOGGER, message);
        }

        internal static void Warn(string message)
        {
            systemLogger.Warn(LoggerManager.TYPE_SYSTEM_LOGGER, message);
        }

        internal static void Error(string message)
        {
            systemLogger.Error(LoggerManager.TYPE_SYSTEM_LOGGER, message);
        }

        internal static void Fatal(string message)
        {
            systemLogger.Fatal(LoggerManager.TYPE_SYSTEM_LOGGER, message);
        }

        #endregion

        public Logger(string name) : base(name) { }
    }
}