//-----------------------------------------------------------------------------------------
// <copyright file="ContactChooserTask.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class ContactChooserTask : IDisposable
    {
        bool isDisposed = false;
        PhoneApplicationFrame frame;
        PhoneApplicationPage callerPage;
        ContactChooserPage chooserPage;
        string callbackId;

        static Uri uri = new System.Uri(
            "/CloudPact.MowblyFramework;component/Core/Utils/Contacts/ContactChooserPage.xaml", 
            UriKind.Relative);

        // OnCompletedEvent
        public event EventHandler<ContactChooserTaskEventArgs> OnCompleted = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callbackId">Callback id from JS layer to notify when task completes</param>
        public ContactChooserTask(string callbackId)
        {
            this.callbackId = callbackId;
        }

        /// <summary>
        /// Launches the Contact chooser page
        /// </summary>
        public void Show()
        {
            // Get the page that launches the ContactChooser
            frame = Application.Current.RootVisual as PhoneApplicationFrame;
            callerPage = frame.Content as PhoneApplicationPage;

            // Register Navigated handler
            frame.Navigated += OnNavigated;

            // Open contact chooser
            frame.Navigate(uri);
        }

        #region Event handlers

        // Event handler for navigation
        void OnNavigated(object sender, NavigationEventArgs e)
        {
            try
            {
                Mowbly.AppNavigatedToInternalPage = true;
                if (e.Content == callerPage)
                {
                    // Navigation to caller page. Dispose off stuff.
                    Dispose();
                }
                else if (e.Content is ContactChooserPage)
                {
                    // Subscribe to OnCompleted event
                    chooserPage = e.Content as ContactChooserPage;
                    chooserPage.OnCompleted += OnChooseCompleted;

                    // Load contacts from device
                    chooserPage.LoadContacts();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to view contact page .Reason: " + ex.Message);
            }
        }

        public void OnChooseCompleted(object sender, ContactChooserTaskEventArgs cr)
        {
            if (this.OnCompleted != null)
            {
                cr.CallbackId = this.callbackId;
                OnCompleted(sender, cr);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    chooserPage.OnCompleted -= OnChooseCompleted;
                    frame.Navigated -= OnNavigated;
                    frame = null;
                    callerPage = null;
                    chooserPage = null;

                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }

    /// <summary>
    /// ContactChooserTaskEventArgs
    /// </summary>
    public class ContactChooserTaskEventArgs : TaskEventArgs
    {
        public Contact Contact { get; set; }
        public string CallbackId { get; set; }
    }
}