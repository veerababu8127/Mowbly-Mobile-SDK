//-----------------------------------------------------------------------------------------
// <copyright file="Network.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Network : Feature
    {
         #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_NETWORK; } }

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
                case "getActiveNetwork":
                    Tuple<NetworkType, string> activeNetwork = GetActiveNetwork();
                    InvokeCallbackJavascript(callbackId, new MethodResult
                        {
                            Code = (int)activeNetwork.Item1,
                            Result = activeNetwork.Item2
                        });
                    break;
                case "isHostReachable":
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        string url = message.Args[0] as string;
                        long timeout = (long)message.Args[1];
                        try
                        {
                            using (HttpClient httpClient = new HttpClient())
                            {
                                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                                HttpResponseMessage response = await httpClient.GetAsync(new Uri(url));
                                int code = MethodResult.SUCCESS_CODE;
                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    code = MethodResult.FAILURE_CODE;
                                }
                                InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = code,
                                    Result = (response.StatusCode != HttpStatusCode.NotFound)
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error checking reachability of host. Reason - " + e.Message);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = MethodResult.FAILURE_CODE,
                                    Error = new MethodError
                                    {
                                        Message = e.Message
                                    }
                                });
                        }
                    }
                    else
                    {
                        string error = Mowbly.GetString(Constants.STRING_NO_CONNECTIVITY);
                        InvokeCallbackJavascript(callbackId, new MethodResult
                        {
                            Code = MethodResult.FAILURE_CODE,
                            Error = new MethodError
                            {
                                Message = error
                            }
                        });
                    }
                    break;
                default:
                    Logger.Error("Feature " + Name + " does not support method " + message.Method);
                    break;
            }
        }

        #endregion

        #region Network

        /// <summary>
        /// Returns the active network type and its name
        /// </summary>
        /// <returns>Tuple containing the active network type and name</returns>
        private Tuple<NetworkType, string> GetActiveNetwork()
        {
            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net =
                Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            return Tuple.Create(NetworkManager.Instance.GetActiveNetwork(), net.ToString());
        }

        #endregion
    }
}