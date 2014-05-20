//-----------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Log : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_LOG; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            switch (message.Method)
            {
                case "log":
                    string logMessage = message.Args[0] as string;
                    string logTag = message.Args[1] as string;
                    long severity = (long)message.Args[2];
                    Logger logger = 
                        LoggerManager.Instance.GetLogger(LoggerManager.TYPE_USER_LOGGER);
                    switch (severity)
                    {
                        case (long)LogPriority.Log_Priority.Debug:
                            logger.Debug(logTag, logMessage);
                            break;
                        case (long)LogPriority.Log_Priority.Info:
                            logger.Info(logTag, logMessage);
                            break;
                        case (long)LogPriority.Log_Priority.Warn:
                            logger.Warn(logTag, logMessage);
                            break;
                        case (long)LogPriority.Log_Priority.Error:
                            logger.Error(logTag, logMessage);
                            break;
                        case (long)LogPriority.Log_Priority.Fatal:
                            logger.Fatal(logTag, logMessage);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    Logger.Error("Feature " + Name + " does not support method " + message.Method);
                    break;
            }
            await Task.FromResult(0);
        }

        #endregion
    }
}
