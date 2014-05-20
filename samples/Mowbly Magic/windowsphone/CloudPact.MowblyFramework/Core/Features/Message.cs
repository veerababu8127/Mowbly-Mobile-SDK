//-----------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Ui;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Message : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_MESSAGE; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            
            try
            {
                switch (message.Method)
                {
                    case "sendMail":
                        List<string> toList = ((JToken)message.Args[0]).ToObject<List<string>>();
                        string subject = message.Args[1] as string;
                        string body = message.Args[2] as string;
                        List<string> ccList = ((JToken)message.Args[3]).ToObject<List<string>>();
                        List<string> bccList = ((JToken)message.Args[4]).ToObject<List<string>>();

                        // Create email compose task and display
                        EmailComposeTask emailComposeTask = new EmailComposeTask();
                        emailComposeTask.To = String.Join("; ", toList);
                        emailComposeTask.Subject = subject;
                        emailComposeTask.Body = body;
                        emailComposeTask.Cc = String.Join("; ", ccList);
                        emailComposeTask.Bcc = String.Join("; ", bccList);
                        UiDispatcher.BeginInvoke(() =>
                            {
                                emailComposeTask.Show();
                            });

                        // Set app navigated to external page
                        Mowbly.AppNavigatedToExternalPage = true;

                        break;
                    case "sendText":
                    case "sendData":
                        // Create sms compose task and show
                        List<string> phoneNumbers = ((JToken)message.Args[0]).ToObject<List<string>>();
                        string text = message.Args[1] as string;

                        SmsComposeTask smsComposeTask = new SmsComposeTask();
                        smsComposeTask.To = String.Join(";", phoneNumbers);
                        smsComposeTask.Body = text;
                        UiDispatcher.BeginInvoke(() =>
                        {
                            smsComposeTask.Show();
                        });

                        // Set app navigated to external page
                        Mowbly.AppNavigatedToExternalPage = true;

                        break;
                    default:
                        Logger.Error("Feature " + Name + " does not support method " + message.Method);
                        break;
                }
            }
            catch (Exception ce)
            {
                Logger.Error("Exception occured. Reason - " + ce.Message);
            }
            await Task.FromResult(0);
        }

        #endregion
    }
}