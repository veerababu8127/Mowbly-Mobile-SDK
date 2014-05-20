//-----------------------------------------------------------------------------------------
// <copyright file="PreferencesManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using System;
using System.IO.IsolatedStorage;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class PreferencesManager
    {
        static readonly object settingsLock = new Object();

        static IsolatedStorageSettings settings = 
            IsolatedStorageSettings.ApplicationSettings;

        #region Singleton

        static readonly Lazy<PreferencesManager> instance =
            new Lazy<PreferencesManager>(() => new PreferencesManager());

        PreferencesManager() { }

        internal static PreferencesManager Instance { get { return instance.Value; } }

        #endregion

        #region Accessors

        /// <summary>
        /// Tells if a value for the specified key is avialable in the preferences
        /// </summary>
        /// <param name="key">Key name of the preference</param>
        /// <returns>True, if a value for key is available; False, otherwise</returns>
        internal bool Contains(string key)
        {
            return (!String.IsNullOrWhiteSpace(key) && settings.Contains(key));
        }

        /// <summary>
        /// Returns the value for the specified preference key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key name of the preference</param>
        /// <param name="returnDefaultValue">Tells if the default value of the 
        /// type should be returned if the value is not present for the 
        /// preference key</param>
        /// <returns>Value of the specified preference key</returns>
        internal T Get<T>(string key, bool returnDefaultValue = false)
        {
            object value = null;
            if (!String.IsNullOrWhiteSpace(key))
            {
                if (settings.Contains(key))
                {
                    value = (T)settings[key];
                }
                else
                {
                    if (returnDefaultValue)
                    {
                        value = default(T);
                    }
                    else
                    {
                        Logger.Error("Preference [" + key + "] not available. Returning default value.");
                        value = default(T);
                    }
                }
            }
            return (T)value;
        }

        /// <summary>
        /// Puts the value in the specified preference key and persists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key name of the preference</param>
        /// <param name="value">Value of the preference</param>
        internal void Put<T>(string key, T value)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                settings[key] = value;
                Save();
            }
        }

        /// <summary>
        /// Clears the value of the specified preference key and persists
        /// </summary>
        /// <param name="key">Key name of the preference</param>
        internal void Clear(string key)
        {
            if (!String.IsNullOrWhiteSpace(key) && settings.Contains(key))
            {
                settings.Remove(key);
                Save();
            }
        }


        #endregion

        #region Utils

        void Save()
        {
            lock (settingsLock)
            {
                try
                {
                    settings.Save();
                }
                catch (IsolatedStorageException e)
                {
                    Logger.Fatal("Error persisting app settings. Reason - " + e.Message);
                }
                catch(Exception e)
                {
                    Logger.Fatal("Error persisting app settings. Reason - " + e.Message);
                }
            }
        }

        #endregion
    }
}