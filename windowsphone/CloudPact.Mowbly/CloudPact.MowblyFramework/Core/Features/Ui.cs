//-----------------------------------------------------------------------------------------
// <copyright file="Ui.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using Coding4Fun.Toolkit.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Ui : Feature
    {
        private enum UiControl
        {
            None = -1,
            Alert = 0,
            Confirm,
            Progress,
            Toast
        }

        private const int DURATION_TOAST_LONG = 3500;
        private const int DURATION_TOAST_SHORT = 2000;
        private const string KEY_TITLE = "title";
        private const string KEY_MESSAGE = "message";
        private const string KEY_CALLBACK_ID = "callbackId";
        private const string BUTTON_LABEL_OK = "ok";

        private static UiControl activeControl = UiControl.None;
        private static MessagePrompt prompt;

        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_UI; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            UiDispatcher.BeginInvoke(() =>
            {
                switch (message.Method)
                {
                    case "alert":

                        if (activeControl == UiControl.None)
                        {
                            // Read args
                            string alertOptionsStr = message.Args[0] as string;
                            JObject alertOptions = JObject.Parse(alertOptionsStr);
                            string alertTitle = (string)alertOptions[KEY_TITLE];
                            string alertMsg = (string)alertOptions[KEY_MESSAGE];
                            string alertCallbackId = (string)alertOptions[KEY_CALLBACK_ID];

                            // Create alert prompt
                            MessagePrompt alertPrompt = new MessagePrompt();
                            alertPrompt.Title = alertTitle;
                            alertPrompt.Message = alertMsg;

                            // Subscribe to OnCompleted event
                            EventHandler<PopUpEventArgs<string, PopUpResult>> onAlertPromptCompleted = null;
                            onAlertPromptCompleted = (sender, e) =>
                            {
                                // Set active control none
                                if (activeControl != UiControl.Progress)
                                {
                                    activeControl = UiControl.None;
                                }
                                prompt = null;

                                UiDispatcher.BeginInvoke(() =>
                                {
                                    // Notify JS layer
                                    InvokeCallbackJavascript(alertCallbackId, new MethodResult());

                                    // Unsubscrivbe from OnCompleted event
                                    alertPrompt.Completed -= onAlertPromptCompleted;
                                });
                            };
                            alertPrompt.Completed += onAlertPromptCompleted;

                            // Show the alert prompt
                            alertPrompt.Show();

                            // Set active control
                            activeControl = UiControl.Alert;
                            prompt = alertPrompt;
                        }
                        break;
                    
                    case "confirm":

                        if (activeControl == UiControl.None)
                        {
                            // Read args
                            string confirmOptionsStr = message.Args[0] as string;
                            JObject confirmOptions = JObject.Parse(confirmOptionsStr);
                            List<Dictionary<string, string>> confirmButtons =
                                confirmOptions["buttons"].ToObject<List<Dictionary<string, string>>>();
                            if (confirmButtons.Count >= 2)
                            {
                                string confirmTitle = (string)confirmOptions[KEY_TITLE];
                                string confirmMsg = (string)confirmOptions[KEY_MESSAGE];
                                string confirmCallbackId = (string)confirmOptions[KEY_CALLBACK_ID];

                                // Create confirm prompt
                                MessagePrompt confirmPrompt = new MessagePrompt();
                                confirmPrompt.Title = confirmTitle;
                                confirmPrompt.Message = confirmMsg;
                                confirmPrompt.IsCancelVisible = true;

                                // Subscribe to OnCompleted event
                                // Required if the user cancels the confirm dialog using device back key press
                                EventHandler<PopUpEventArgs<string, PopUpResult>> onConfirmPromptCompleted = null;
                                onConfirmPromptCompleted = (sender, e) =>
                                {
                                    // Set active control none
                                    activeControl = UiControl.None;
                                    prompt = null;

                                    UiDispatcher.BeginInvoke(() =>
                                    {
                                        // Notify JS layer
                                        InvokeCallbackJavascript(confirmCallbackId, -1);

                                        // Unsubscribe from OnCompleted event
                                        confirmPrompt.Completed -= onConfirmPromptCompleted;
                                    });
                                };
                                confirmPrompt.Completed += onConfirmPromptCompleted;

                                // Function to return the button click handler encapsulating the index of the button
                                Func<int, RoutedEventHandler> GetButtonClickHandler = (btnIndex) =>
                                {
                                    return new RoutedEventHandler((o, args) =>
                                    {
                                        // Unsubscribe from OnCompleted event
                                        // Not required as JS layer will be notified from here
                                        confirmPrompt.Completed -= onConfirmPromptCompleted;

                                        // Notify JS layer
                                        InvokeCallbackJavascript(confirmCallbackId, btnIndex);

                                        // Hide the confirm prompt
                                        confirmPrompt.Hide();

                                        // Set active control none
                                        activeControl = UiControl.None;
                                        prompt = null;
                                    });
                                };

                                // Remove the default buttons
                                confirmPrompt.ActionPopUpButtons.Clear();

                                // Create confirm buttons and add it to confirm prompt
                                int index = 0;
                                foreach (Dictionary<string, string> buttonDict in confirmButtons)
                                {
                                    Button button = new Button { Content = buttonDict["label"] };
                                    button.Click += GetButtonClickHandler(index);
                                    confirmPrompt.ActionPopUpButtons.Add(button);
                                    index++;
                                }

                                // Show the alert prompt
                                confirmPrompt.Show();

                                // Set active control
                                activeControl = UiControl.Confirm;
                                prompt = confirmPrompt;
                            }
                        }
                        break;
                    
                    case "hideProgress":

                        if (activeControl == UiControl.Progress)
                        {
                            CloudPact.MowblyFramework.Core.Controls.ProgressOverlay.Instance.Hide();

                            // Set active control none
                            activeControl = UiControl.None;
                        }
                        break;
                    
                    case "showProgress":

                        if (activeControl != UiControl.Progress)
                        {
                            string progressMsg = message.Args[1] as string;
                            CloudPact.MowblyFramework.Core.Controls.ProgressOverlay.Instance.Show(progressMsg);

                            // Set active control
                            activeControl = UiControl.Progress;
                        }
                        break;

                    case "toast":

                        if (activeControl == UiControl.None)
                        {
                            string toastMsg = message.Args[0] as string;
                            int duration = DURATION_TOAST_LONG;
                            if (message.Args.Length > 1)
                            {
                                duration = Convert.ToInt32(message.Args[1]);
                                duration = (duration == 0) ? DURATION_TOAST_SHORT :
                                    DURATION_TOAST_LONG;
                            }
                            ToastPrompt toastPrompt = new ToastPrompt
                            {
                                Message = toastMsg,
                                MillisecondsUntilHidden = duration
                            };
                            toastPrompt.Show();
                        }
                        break;
                   
                    default:
                        Logger.Error("Feature " + Name + " does not support method " + message.Method);
                        break;
                }
            });
            await Task.FromResult(0);
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Ui() : base()
        {
            // Subscribe to device back key press event
            MowblyClientManager.Instance.OnBackKeyPress += OnBackKeyPress;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Ui()
        {
            Dispose();
        }

        // Invoked when device back key pressed
        public void OnBackKeyPress(object sender, CancelEventArgs e)
        {
            if (!e.Cancel && activeControl != UiControl.None)
            {
                if (activeControl == UiControl.Alert ||
                    activeControl == UiControl.Confirm)
                {
                    prompt.Hide();
                } else if (activeControl == UiControl.Progress)
                {
                    // Do nothing. Progress bar is not cancelable.
                }

                // Set event handled
                e.Cancel = true;
            }
        }

        #region IDisposable

        public override void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    base.Dispose();

                    // Clear prompt object
                    prompt = null;

                    // Unsubscribe from device backpress event
                    MowblyClientManager.Instance.OnBackKeyPress -= OnBackKeyPress;

                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }
}
