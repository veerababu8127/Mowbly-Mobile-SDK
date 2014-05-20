//-----------------------------------------------------------------------------------------
// <copyright file="ContactDetailsPage.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CloudPact.MowblyFramework.Core.Utils
{
    public partial class ContactDetailsPage : PhoneApplicationPage
    {
        private static string KEY_PARAM = "param";

        private static string KEY_TYPE = "type";

        public ContactDetailsPage()
        {
            InitializeComponent();
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            string param = String.Empty;
            string type = String.Empty;
            NavigationContext.QueryString.TryGetValue(KEY_PARAM, out param);
            NavigationContext.QueryString.TryGetValue(KEY_TYPE, out type);

            if (param != String.Empty && type != String.Empty)
            {
                Contacts contacts = new Contacts();
                contacts.SearchCompleted += OnContactsSearchCompleted;
                switch (type)
                {
                    case "phone":
                        contacts.SearchAsync(param, FilterKind.PhoneNumber, null);
                        break;
                    case "name":
                        contacts.SearchAsync(param, FilterKind.DisplayName, null);
                        break;
                    case "email":
                        contacts.SearchAsync(param, FilterKind.EmailAddress, null);
                        break;
                    case "none":
                        ContactName.Text = Mowbly.GetString(Constants.STRING_CONTACT_SEARCH_PARAM_MISSING);
                        break;
                }
            }
            else
            {
                ContactName.Text = Mowbly.GetString(Constants.STRING_CONTACT_SEARCH_PARAM_MISSING);
            }
        }

        private void OnContactsSearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            try
            {
                IEnumerator<Contact> contacts = e.Results.GetEnumerator();
                if (contacts.MoveNext())
                {
                    Contact contact = contacts.Current;
                    List<ContactDetail> details = new List<ContactDetail>();
                    ContactDetail.Type type;
                    string value;
                    string kind;

                    // Name & NickName
                    CompleteName name = contact.CompleteName;
                    if (name != null)
                    {
                        string cName = name.Title + " " + name.FirstName + " " + name.MiddleName + " " + name.LastName;
                        ContactName.Text = cName.ToUpper();
                    }
                    else
                    {
                        ContactName.Text = contact.DisplayName;
                    }

                    // Photos
                    Stream stream = contact.GetPicture();
                    if (stream == null)
                    {
                        Picture.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        BitmapImage img = new BitmapImage();
                        img.SetSource(stream);
                        Picture.Source = img;
                    }

                    // Phones
                    type = ContactDetail.Type.Phone;
                    foreach (ContactPhoneNumber phoneNumber in contact.PhoneNumbers)
                    {
                        if (phoneNumber.PhoneNumber != null)
                        {
                            value = phoneNumber.PhoneNumber;
                            kind = phoneNumber.Kind.ToString();
                            details.Add(new ContactDetail(type, value, kind));

                            // Sms, if mobile
                            if (phoneNumber.Kind == PhoneNumberKind.Mobile)
                            {
                                details.Add(new ContactDetail(ContactDetail.Type.Sms, value, kind));
                            }
                        }
                    }

                    // Urls
                    type = ContactDetail.Type.Website;
                    foreach (string website in contact.Websites)
                    {
                        value = website;
                        kind = String.Empty;
                    }

                    // Emails
                    type = ContactDetail.Type.Email;
                    foreach (ContactEmailAddress email in contact.EmailAddresses)
                    {
                        value = email.EmailAddress;
                        kind = email.Kind.ToString().ToLower();

                        details.Add(new ContactDetail(type, value, kind));
                    }

                    // Organizations
                    IEnumerator<ContactCompanyInformation> companies = contact.Companies.GetEnumerator();
                    ContactCompanyInformation company;
                    while (companies.MoveNext())
                    {
                        company = (ContactCompanyInformation)companies.Current;

                        if (company.CompanyName != null)
                        {
                            type = ContactDetail.Type.Company;
                            value = company.CompanyName;
                            kind = String.Empty;
                            details.Add(new ContactDetail(type, value, kind));
                        }

                        if (company.JobTitle != null)
                        {
                            type = ContactDetail.Type.JobTitle;
                            value = company.JobTitle;
                            kind = String.Empty;
                            details.Add(new ContactDetail(type, value, kind));
                        }
                    }

                    // Address
                    type = ContactDetail.Type.Address;
                    foreach (ContactAddress address in contact.Addresses)
                    {
                        CivicAddress ca = address.PhysicalAddress;
                        // Street, City, PostalCode, Region, Country
                        value = ca.AddressLine1 + " " + ca.AddressLine2 + " " + ca.Building + "\n" +
                            ca.City + " " + ca.StateProvince + " " + ca.PostalCode + "\n" +
                            ca.CountryRegion;
                        kind = address.Kind.ToString();

                        details.Add(new ContactDetail(type, value, kind));
                    }

                    // Display the data
                    DetailsList.DataContext = details;
                }
                else
                {
                    ContactName.Text = Mowbly.GetString(Constants.STRING_CONTACT_NOT_FOUND);
                    ContactAccounts.Text = "";
                    Picture.Visibility = System.Windows.Visibility.Collapsed;
                    DetailsList.DataContext = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to view contact.Reason: " + ex.Message);
            }
        }

        // Invoked when user taps on a contact detail. Corresponding action is triggered for the detail.
        private void DetailsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContactDetail contactDetail = ((sender as ListBox).SelectedValue as ContactDetail);
            Logger.Debug("Viewing contact detail "+ contactDetail.Value);
            contactDetail.Action();
        }
    }
}