//-----------------------------------------------------------------------------------------
// <copyright file="Device.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Device : Feature
    {
        const double DEFAULT_MEMORY_VALUE = 0.0;

        const string KEY_AVAILABLE_MEMORY = "availableInternalMemory";
        const string KEY_TOTAL_MEMORY = "totalInternalMemory";
        const string KEY_AVAILABLE_EXTERNAL_MEMORY = "availableExternalMemory";
        const string KEY_TOTAL_EXTERNAL_MEMORY = "totalExternalMemory";
        const string KEY_APPLICATION_MEMORY = "applicationMemory";

        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_DEVICE; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            string callbackId = message.CallbackId;
            switch (message.Method)
            {
                case "getDeviceId":
                    
                    InvokeCallbackJavascript(callbackId, new MethodResult { Result = DeviceManager.Instance.DeviceId });
                    break;

                case "getMemoryStatus":

                    Dictionary<string, string> memory = new Dictionary<string, string>();
                    double availInternalMemory = DEFAULT_MEMORY_VALUE;
                    double applicationMemory = DEFAULT_MEMORY_VALUE;
                    try
                    {
                        using (IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            //Available memory in the internal storage.
                            availInternalMemory = Math.Round((double)Storage.AvailableFreeSpace / (1024 * 1024), 2);

                            //memory used by application
                            applicationMemory = Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage;
                            applicationMemory = Math.Round((double)applicationMemory / (1024 * 1024), 2);

                            // SD card not supported
                            memory.Add(KEY_AVAILABLE_MEMORY, availInternalMemory.ToString("#.00MB"));
                            memory.Add(KEY_TOTAL_MEMORY, Constants.STRING_UNAVAILABLE);
                            memory.Add(KEY_AVAILABLE_EXTERNAL_MEMORY, Constants.STRING_UNAVAILABLE);
                            memory.Add(KEY_TOTAL_EXTERNAL_MEMORY, Constants.STRING_UNAVAILABLE);
                            memory.Add(KEY_APPLICATION_MEMORY, applicationMemory.ToString("#.00MB"));
                        }
                    }
                    catch
                    {
                        memory.Add(KEY_AVAILABLE_MEMORY, availInternalMemory.ToString("#.00MB"));
                        memory.Add(KEY_TOTAL_MEMORY, Constants.STRING_UNAVAILABLE);
                        memory.Add(KEY_AVAILABLE_EXTERNAL_MEMORY, Constants.STRING_UNAVAILABLE);
                        memory.Add(KEY_TOTAL_EXTERNAL_MEMORY, Constants.STRING_UNAVAILABLE);
                        memory.Add(KEY_APPLICATION_MEMORY, applicationMemory.ToString("#.00MB"));
                    }
                    InvokeCallbackJavascript(callbackId, new MethodResult { Result = memory });
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
