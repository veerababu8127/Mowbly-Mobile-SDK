//-----------------------------------------------------------------------------------------
// <copyright file="Framework.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Controls;
using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace CloudPact.MowblyFramework.Core.Features
{
    class Framework : Feature
    {
        # region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_FRAMEWORK; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            PageManager pageMgr = PageManager.Instance;
            string msg;
            string callbackId = message.CallbackId;
            switch (message.Method)
            {
                case "broadcastMessage":
                    msg = message.Args[0] as string; 
                    await pageMgr.BroadcastMessageAsync(msg);
                    break;
                case "closeApplication":
                    UiDispatcher.BeginInvoke(() =>
                        {
                            pageMgr.ClosePage();
                        });
                    break;                
                case "launchApplication":
                    string name = message.Args[0] as string;
                    string url = message.Args[1] as string;
                    string data = message.Args[2] as string;
                    Dictionary<string, object> options = 
                        ((JToken)message.Args[3]).ToObject<Dictionary<string, object>>();
                    bool isExternalUrl = false;
                    try
                    {
                        Uri uri = null;
                        Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
                        isExternalUrl = uri.IsAbsoluteUri;
                    }
                    catch
                    {
                        // Exception in parsing url; consider it external
                        isExternalUrl = true;
                    }

                    UiDispatcher.BeginInvoke(() =>
                        {
                            if (isExternalUrl)
                            {
                                try
                                {
                                    WebBrowserTask task = new WebBrowserTask();
                                    task.Uri = new Uri(url, UriKind.Absolute);
                                    task.Show();
                                    return;
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Error opening page " + name + " . Reason - " + e.Message);
                                }
                            }
                            else
                            {
                                pageMgr.OpenPage(name, url, data, options);
                            }
                        }); 
                   break;
                case "openExternal":
                    // TODO: Check support for external apps
                    break;
                case "postMessage":
                    string pageName = message.Args[0] as string;
                    msg = message.Args[1] as string;
                    await pageMgr.PostMessageToPageAsync(pageName, msg);
                    break;
                case "setPageResult":
                    string result = message.Args[0] as string;
                    pageMgr.ActivePage.setResult(result);
                    break;

                default:
                    Logger.Error("Feature " + Name + " does not support method " + message.Method);
                    break;
            }
        }

        #endregion
    }
}