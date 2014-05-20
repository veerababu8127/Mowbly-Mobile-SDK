//-----------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CloudPact.Mowbly
{
    public partial class MainPage : PhoneApplicationPage, IMowblyPhoneApplicationPage
    {
        private bool isNewInstance = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set as new page instance
            isNewInstance = true;

            Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            bool UAexists = MowblyClientManager.Instance.DoesUAexists();
            if (!UAexists)
            {
                UserAgentHelper.GetUserAgent(
                LayoutRoot,
                userAgent =>
                {
                    MowblyClientManager.Instance.PutUA(userAgent);
                });
            }
        }

        public static class UserAgentHelper
        {
            private const string Html =
                @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
 
        <html>
        <head>
        <script language=""javascript"" type=""text/javascript"">
            function notifyUA() {
               window.external.notify(navigator.userAgent);
            }
        </script>
        </head>
        <body onload=""notifyUA();""></body>
        </html>";

            public static void GetUserAgent(Panel rootElement, Action<string> callback)
            {
                var browser = new Microsoft.Phone.Controls.WebBrowser();
                browser.IsScriptEnabled = true;
                browser.Visibility = Visibility.Collapsed;
                browser.Loaded += (sender, args) => browser.NavigateToString(Html);
                browser.ScriptNotify += (sender, args) =>
                {
                    string userAgent = args.Value;
                    rootElement.Children.Remove(browser);
                    callback(userAgent);
                };
                rootElement.Children.Add(browser);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Notify MowblyClientManager
            MowblyClientManager.Instance.NavigatedToMainPage(this, e);  

            // Set new page instance false
            isNewInstance = false;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // Notify MowblyClientManager
            MowblyClientManager.Instance.BackKeyPress(e);
        }

        #region IMowblyPhoneApplicationPage

        public bool IsNewInstance { get { return isNewInstance; } }

        public Grid GetContentPanel()
        {
            return ContentPanel;
        }

        public Canvas GetViewFinderCanvas()
        {
            return viewfinderCanvas;
        }

        public void HideProgress()
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            this.ProgressBar.IsIndeterminate = false;
        }

        public void ShowProgress(string message)
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            this.ProgressBar.IsIndeterminate = true;
            this.ProgressText.Text = message;
        }

        #endregion
    }
}