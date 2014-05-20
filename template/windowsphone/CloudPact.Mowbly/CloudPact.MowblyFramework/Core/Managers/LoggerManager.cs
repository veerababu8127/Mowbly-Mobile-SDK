//-----------------------------------------------------------------------------------------
// <copyright file="LoggerManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Log.Handler;
using CloudPact.MowblyFramework.Core.Utils;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class LoggerManager
    {
        internal static string TYPE_SYSTEM_LOGGER = "system";

        internal static string TYPE_USER_LOGGER = "user";

        string UserAgent;

        Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

        #region Singleton

        static readonly Lazy<LoggerManager> instance =
            new Lazy<LoggerManager>(() => new LoggerManager());

        LoggerManager() {
            // Initialize Loggers
            ConsoleLogHandler cHandler = new ConsoleLogHandler();
            FileLogHandler fHandler = new FileLogHandler(Mowbly.LogFile);
            DatabaseLogHandler dHandler = new DatabaseLogHandler(
                Mowbly.GetProperty<string>(Constants.PROPERTY_LOGS_DB),
                float.Parse(Mowbly.GetProperty<string>(Constants.PROPERTY_LOGS_DB_VERSION)),
                Mowbly.LogDatabaseFile,
                System.String.Empty);

            // System Logger
            Logger systemLogger = GetLogger(TYPE_SYSTEM_LOGGER);
            systemLogger.AddHandler(fHandler);
            systemLogger.AddHandler(dHandler);

            // User Logger
            Logger userLogger = GetLogger(TYPE_USER_LOGGER);
            userLogger.AddHandler(fHandler);
            userLogger.AddHandler(dHandler);

#if DEBUG
            systemLogger.AddHandler(cHandler);
            userLogger.AddHandler(cHandler);
#endif

        }

        internal static LoggerManager Instance { get { return instance.Value; } }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the Logger<see cref="Logger"/> object for the specified logger name
        /// </summary>
        /// <param name="loggerName">Name of the logger</param>
        /// <returns>Logger<see cref="Logger"> object for the specified logger name</returns>
        internal Logger GetLogger(string loggerName)
        {
            Logger logger;
            if (loggers.ContainsKey(loggerName))
            {
                logger = loggers[loggerName];
            }
            else
            {
                logger = new Logger(loggerName);
                loggers[loggerName] = logger;
            }

            return logger;
        }

        
        /// <summary>
        /// Reset loggers
        /// </summary>
        internal void Reset()
        {
            GetLogger(TYPE_USER_LOGGER).Reset();
            GetLogger(TYPE_SYSTEM_LOGGER).Reset();
        }

        /// <summary>
        /// Start loggers
        /// </summary>
        internal void Start()
        {
            GetLogger(TYPE_USER_LOGGER).Start();
            GetLogger(TYPE_SYSTEM_LOGGER).Start();
        }

        /// <summary>
        /// Shutdown loggers
        /// </summary>
        internal void Shutdown()
        {
            GetLogger(TYPE_USER_LOGGER).Shutdown();
            GetLogger(TYPE_SYSTEM_LOGGER).Shutdown();
        }

        #endregion        
    }
}
