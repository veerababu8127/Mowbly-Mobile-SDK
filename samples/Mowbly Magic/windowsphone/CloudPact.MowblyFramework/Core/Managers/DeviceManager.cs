//-----------------------------------------------------------------------------------------
// <copyright file="PreferencesManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Info;
using System;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class DeviceManager
    {
        const string DEVICE_UNIQUE_ID_KEY = "DeviceUniqueId";

        #region Singleton

        static readonly Lazy<DeviceManager> instance =
            new Lazy<DeviceManager>(() => new DeviceManager());

        DeviceManager() { }

        internal static DeviceManager Instance { get { return instance.Value; } }

        #endregion

        #region DeviceManager

        /// <summary>
        /// Returns the unique ID of the device
        /// </summary>
        internal string DeviceId
        {
            get
            {
                byte[] id = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue(DEVICE_UNIQUE_ID_KEY);
                return Convert.ToBase64String(id);
            }
        }

        /// <summary>
        /// Manufacturer of the device
        /// </summary>
        internal string DeviceManufacturer
        {
            get { return DeviceStatus.DeviceManufacturer; }
        }

        /// <summary>
        /// Name of the device
        /// </summary>
        internal string DeviceName
        {
            get { return DeviceStatus.DeviceName; }
        }

        #endregion
    }
}
