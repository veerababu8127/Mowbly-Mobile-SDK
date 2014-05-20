//-----------------------------------------------------------------------------------------
// <copyright file="Preferences.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Preferences : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_PREFERENCES; } }



        internal void SetGlobalPreferences(string gPreferences)
        {
            Dictionary<string, object> preferences =
                (PreferencesManager.Instance.Contains(Constants.KEY_GLOBAL)) ?
                PreferencesManager.Instance.Get<Dictionary<string, object>>(Constants.KEY_GLOBAL) :
                new Dictionary<string, object>();
            string prefKey = String.Format("{0}",Constants.KEY_GLOBAL);
            preferences[prefKey] = gPreferences;
            PreferencesManager.Instance.Put<Dictionary<string, object>>(
                Constants.KEY_GLOBAL, preferences);
        }

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
                case "commit":
                    string gPreferences = message.Args[0] as string;
                    SetGlobalPreferences(gPreferences);
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
