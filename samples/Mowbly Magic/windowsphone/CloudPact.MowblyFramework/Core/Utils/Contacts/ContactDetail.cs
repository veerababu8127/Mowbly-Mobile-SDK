//-----------------------------------------------------------------------------------------
// <copyright file="ContactDetail.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class ContactDetail
    {
        // Enumeration of available contact detail types
        public enum Type
        {
            Address,
            Company,
            Email,
            JobTitle,
            Phone,
            Sms,
            Website
        }

        // Dictionary of actions for each detail type
        static Dictionary<Type, string> descriptions = new Dictionary<Type, string>()
        {
            {Type.Address, "{0} address"},
            {Type.Company, "company"},
            {Type.Email, "send email"},
            {Type.JobTitle, "job title"},
            {Type.Phone, "call {0}"},
            {Type.Sms, "text"},
            {Type.Website, "view website"}
        };

        // Type of the contact detail
        private Type type;

        // Description of the contact detail
        string description;
        public string Description { get { return description; } }

        // Value of the contact detail
        string value;
        public string Value { get { return value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the contact detail</param>
        /// <param name="value">Value of the contact detail</param>
        /// <param name="kind">Kind of the contact detail e.x. Home or Work</param>
        public ContactDetail(Type type, string value, string kind)
        {
            this.type = type;
            this.description = String.Format(
                descriptions[type], 
                (kind == null) ? String.Empty : kind.ToLower());
            this.value = value;
        }

        /// <summary>
        /// Initiate action based on the contact detail
        /// </summary>
        public void Action()
        {
            switch (type)
            {
                case Type.Email:
                    EmailComposeTask emailComposeTask = new EmailComposeTask();
                    emailComposeTask.To = value;
                    emailComposeTask.Show();

                    // Set navigated to external page
                    Mowbly.AppNavigatedToExternalPage = true;
                    break;
                
                case Type.Phone:
                    PhoneCallTask phoneCallTask = new PhoneCallTask();
                    phoneCallTask.PhoneNumber = value;
                    phoneCallTask.Show();

                    // Set navigated to external page
                    Mowbly.AppNavigatedToExternalPage = true;
                    break;

                case Type.Sms:
                    SmsComposeTask smsComposeTask = new SmsComposeTask();
                    smsComposeTask.To = value;
                    smsComposeTask.Show();
                    
                    // Set navigated to external page
                    Mowbly.AppNavigatedToExternalPage = true;
                    break;
                
                case Type.Website:
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(value, UriKind.Absolute);
                    webBrowserTask.Show();

                    // Set navigated to external page
                    Mowbly.AppNavigatedToExternalPage = true;
                    break;
            }
        }
    }
}
