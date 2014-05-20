//-----------------------------------------------------------------------------------------
// <copyright file="ContactViewerTask.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class ContactViewerTask
    {
        string param;

        string type;

        static string uriFormat = 
            "/CloudPact.MowblyFramework;component/Core/Utils/Contacts/ContactDetailsPage.xaml?param={0}&type={1}";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">Search parameter for the contact detail</param>
        /// <param name="type">Type of the search parameter</param>
        public ContactViewerTask(string param, string type)
        {
            this.param = param;
            this.type = type;
        }
            
        /// <summary>
        /// Launches the Contact viewer page
        /// </summary>
        public void Show()
        {
            // Get the page that launches the ContactChooser
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;

            // Open contact chooser
            Uri uri = new System.Uri(String.Format(uriFormat, param, type),UriKind.Relative);
            frame.Navigate(uri);
        }
    }
}
