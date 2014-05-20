//-----------------------------------------------------------------------------------------
// <copyright file="FeatureBinder.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class FeatureBinder
    {
        Dictionary<string, string> features = 
            new Dictionary<string,string>();

        Dictionary<string, Feature> featuresMap = 
            new Dictionary<string,Feature>();

        #region Singleton

        static readonly Lazy<FeatureBinder> instance =
            new Lazy<FeatureBinder>(() => new FeatureBinder());

        FeatureBinder()
        {
            // Load features from Properties
            LoadFeatures();
        }

        internal static FeatureBinder Instance { get { return instance.Value; } }

        #endregion

        #region Public methods

        /// <summary>
        /// Invokes the native method call based on the parameters in the specified
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage ">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage ">JSMessage</see> object
        /// </param>
        /// <returns><see cref="System.Threading.Tasks.Task">Task</see></returns>
        internal async Task InvokeAsync(JSMessage message)
        {
            Feature f = GetFeature(message.Feature);
            if (f != null)
            {
                await Task.Run(() =>
                {
                    f.InvokeAsync(message);
                });
            }
            else
            {
                Logger.Error("Feature " + message.Feature + " not available.");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.Feature">feature</see> 
        /// object for the specified feature name
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns>
        /// <see cref="CloudPact.MowblyFramework.Core.Features.Feature">Feature</see> object
        /// </returns>
        Feature GetFeature(string featureName)
        {
            Feature f;
            if (featuresMap.ContainsKey(featureName))
            {
                f = featuresMap[featureName];
            }
            else
            {
                f = Activator.CreateInstance(Type.GetType(features[featureName]), new object[] { }) as Feature;
                featuresMap[featureName] = f;
            }
            return f;
        }

        /// <summary>
        /// Loads the features listed in the 'features' property.
        /// The properties should be name value pairs separated by "=", without any spaces around separator.
        /// The value for property is always cast to string.
        /// </summary>
        void LoadFeatures()
        {
            try
            {
                StringReader reader = new StringReader(Mowbly.GetProperty<string>(Constants.PROPERTY_FEATURES));
                if (reader != null)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Read name=value pair. Ignore comments (#) and empty lines.
                        if (!line.StartsWith("#") && !line.Equals(String.Empty))
                        {
                            line = line.Trim();
                            string[] property = line.Split('=');
                            features.Add(property[0], string.Join("=", property.Skip(1).ToArray()));
                            //Logger.Warn("Key: " + property[0] + "Value" + string.Join("=", property.Skip(1).ToArray()));
                        }
                    }
                }
                else
                {
                    Logger.Error("Could not load features. Reason - Error reading features property");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not load features. Reason - " + e.Message);
            }
        }

        #endregion
    }
}
