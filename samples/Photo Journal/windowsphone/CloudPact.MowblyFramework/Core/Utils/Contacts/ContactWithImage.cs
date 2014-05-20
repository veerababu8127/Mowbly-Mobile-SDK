//-----------------------------------------------------------------------------------------
// <copyright file="ContactWithImage.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
// <reference>http://lukencode.com/2011/12/01/windows-phone-mango-contact-chooser-task/</reference>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.UserData;
using System.Windows.Media.Imaging;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class ContactWithImage
    {
        internal Contact Contact { get; private set; }

        /// <summary>
        /// Display name of the contact
        /// </summary>
        public string DisplayName { get { return Contact.DisplayName; } }

        /// <summary>
        /// Picture of the contact
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                var bmp = new BitmapImage();
                var img = Contact.GetPicture();

                if (img == null)
                    return null;

                bmp.SetSource(img);
                return bmp;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContactWithImage() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contact">
        /// <see cref="Microsoft.Phone.UserData.Contact">Contact</see> object
        /// </param>
        public ContactWithImage(Contact contact)
        {
            Contact = contact;
        }
    }
}
