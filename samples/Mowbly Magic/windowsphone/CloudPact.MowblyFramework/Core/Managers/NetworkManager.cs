//-----------------------------------------------------------------------------------------
// <copyright file="NetworkManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Net.NetworkInformation;
using System;
using Windows.Networking.Connectivity;

namespace CloudPact.MowblyFramework.Core.Managers
{
    public enum NetworkType
    {
        None = 0,
        WiFi,
        Cellular,
        Other
    }

    class NetworkManager
    {
        /// <summary>
        /// Event that is raised when the device connects to a network
        /// </summary>
        internal event EventHandler OnNetworkConnected;

        /// <summary>
        /// Event that is raised when the device disconnects from network
        /// </summary>
        internal event EventHandler OnNetworkDisconnected;

        #region Singleton

        static readonly Lazy<NetworkManager> instance =
            new Lazy<NetworkManager>(() => new NetworkManager());

        NetworkManager()
        {
            // Subscribe to network status changes
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }

        ~NetworkManager()
        {
            // Unsubscribe to network status changes
            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
        }

        internal static NetworkManager Instance { get { return instance.Value; } }

        #endregion

        #region Network Manager

        /// <summary>
        /// Returns the type of the active network
        /// </summary>
        /// <returns></returns>
        internal NetworkType GetActiveNetwork()
        {
            NetworkType type;
            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net =
                Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            switch (net)
            {
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                    type = NetworkType.WiFi;
                    break;
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                    type = NetworkType.None;
                    break;
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                    type = NetworkType.Cellular;
                    break;
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.AsymmetricDsl:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Atm:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.BasicIsdn:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet3Megabit:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetFx:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetT:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Fddi:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GenericModem:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GigabitEthernet:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.HighPerformanceSerialBus:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.IPOverAtm:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Isdn:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Loopback:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MultiRateSymmetricDsl:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ppp:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.PrimaryIsdn:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.RateAdaptDsl:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Slip:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.SymmetricDsl:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.TokenRing:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Tunnel:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Unknown:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.VeryHighSpeedDsl:
                default:
                    type = NetworkType.Other;
                    break;
            }

            return type;
        }

        // Invoked when there is a change in network status
        void OnNetworkStatusChanged(object sender)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (OnNetworkConnected != null)
                {
                    OnNetworkConnected(this, new EventArgs());
                }
            }
            else
            {
                if (OnNetworkDisconnected != null)
                {
                    OnNetworkDisconnected(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}
