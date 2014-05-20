//-----------------------------------------------------------------------------------------
// <copyright file="Mowbly.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using System;
using System.IO;
using System.Resources;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core
{
    class Mowbly
    {
        internal static ResourceManager Properties;

        internal static ResourceManager Strings;

        /// <summary>
        /// Tells if the app has navigated to an external app
        /// </summary>
        internal static bool AppNavigatedToExternalPage { get; set; }

        /// <summary>
        /// Tells if the app has navigated to internal non-mowbly page
        /// </summary>
        internal static bool AppNavigatedToInternalPage { get; set; }

        /// <summary>
        /// Synchronization context for tasks
        /// Should be initialized from Ui thread
        /// </summary>
        internal static TaskScheduler UiTaskScheduler { get; set;}

        #region Generic Property / String helpers

        internal static T GetProperty<T>(string key)
        {
            return (T)Properties.GetObject(key);
        }

        internal static string GetString(string key)
        {
            return Strings.GetString(key);
        }

        internal static string GetUserAgent()
        {
            string UserAgent = GetProperty<string>(Constants.PROPERTY_APP_NAME) + "/" + GetProperty<string>(Constants.PROPERTY_APP_VERSION) + " " + GetProperty<string>(Constants.PROPERTY_FRAMEWORK) + "/" + GetProperty<string>(Constants.PROPERTY_FRAMEWORK_VERSION) + " windows/" + Environment.OSVersion.Version.ToString() +" "+ Microsoft.Phone.Info.DeviceStatus.DeviceName;
            return UserAgent;
        }

        #endregion

        #region App filesystem helpers

        internal static string AppDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_APP);
                GetDirectory(path);
                return path;
            }
        }

        internal static string BackupDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_BAK);
                GetDirectory(path);
                return path;
            }
        }

        internal static string CacheDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_CACHE);
                GetDirectory(path);
                return path;
            }
        }

        internal static string DocumentsDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_DOCS);
                GetDirectory(path);
                return path;
            }
        }

        internal static string LogDatabaseFile
        {
            get
            {
                return Path.Combine(Properties.GetString(Constants.PROPERTY_DIR_LOGS),
                    Constants.DIR_DB,
                    String.Concat(
                    Properties.GetString(Constants.PROPERTY_LOGS_DB),
                    Properties.GetString(Constants.PROPERTY_LOGS_DB_VERSION)));
            }
        }

        internal static string LogFile
        {
            get
            {
                return Path.Combine(
                    Properties.GetString(Constants.PROPERTY_DIR_LOGS), 
                    Properties.GetString(Constants.PROPERTY_LOGS_FILE));
            }
        }

        internal static string SharedTransfersDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_SHARED_TRANSFER);
                GetDirectory(path);
                return path;
            }
        }

        

        internal static string TmpDirectory
        {
            get
            {
                string path = Properties.GetString(Constants.PROPERTY_DIR_TMP);
                GetDirectory(path);
                return path;
            }
        }

        private static void GetDirectory(string path)
        {
            try
            {
                FileManager.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Logger.Error("Error getting directory[" + path + "]. Reason - " + e.Message);
            }
        }

        #endregion

        #region Client

        internal static IMowblyPhoneApplicationPage ActivePhoneApplicationPage { get; set; }
        
        #endregion

        #region App managers

        internal static IForegroundTasksManager ForegroundTasksManager
        {
            get
            {
                return (IForegroundTasksManager)CustomForegroundTasksManager.Instance;
            }
        }

        internal static IBackgroundTaskManager BackgroundTaskManager
        {
            get
            {
                return  BackgroundTasksManager.Instance;
            }
        
        }


        #endregion

    }
}