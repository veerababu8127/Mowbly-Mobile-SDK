//-----------------------------------------------------------------------------------------
// <copyright file="ContactChooserPage.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
// <reference>http://lukencode.com/2011/12/01/windows-phone-mango-contact-chooser-task/</reference>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CloudPact.MowblyFramework.Core.Utils
{
    public partial class ContactChooserPage : PhoneApplicationPage
    {
        // Contacts object for search operation
        private Contacts contacts;

        // Groups for contacts, based on the alphabet collection
        private List<ContactsGroup<ContactWithImage>> emptyGroups;
        private static string alphabet = "#abcdefghijklmnopqrstuvwxyz";

        // Event handler that receives the chosen contact
        public event EventHandler<ContactChooserTaskEventArgs> OnCompleted = null;

        public ContactChooserPage()
        {
            InitializeComponent();

            // Create contacts
            contacts = new Contacts();

            // Set progress text and show
            ProgressIndicator.Text = Mowbly.GetString(Constants.STRING_CONTACTS_LOADING);
            ShowProgress();

            // Initialize empty groups
            InitGroups();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ContactChooserPage()
        {
            try
            {
                contacts.SearchCompleted -= new EventHandler<ContactsSearchEventArgs>(OnContactsSearchCompleted);
                ContactsList.ItemsSource = null;
                contacts = null;
            }
            catch { }
        }

        #region Event handlers

        // Back keypress handler
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            HideProgress();
        }

        // Called when the contacts search is completed
        private void OnContactsSearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            if (e.Results.Count() > 0)
            {
                // Group the contacts and set as source items for the ContactsList
                var groupedContacts = from c in e.Results
                                      group c by GetGroupHeader(c) into g
                                      select new ContactsGroup<ContactWithImage>(g.Key.ToString(), g.Select(c => new ContactWithImage(c)).ToList());

                var list = (from t in groupedContacts
                            orderby t.Title
                            select t).ToList();

                ContactsList.ItemsSource = list;
            }

            // Hide progress
            HideProgress();
        }

        // Navigated to event handler, called when page becomes active
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Hide progress
            HideProgress();
        }

        // Key up event handler for the Search contact field.
        private void SearchInput_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Key >= Key.A && e.Key <= Key.Z) ||
                    (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                    e.Key == Key.Space ||
                    e.Key == Key.Back)
                {
                    // Clear the existing list and start the search
                    ClearContacts();

                    // Show progress
                    ShowProgress();

                    // Search
                    contacts.SearchAsync(SearchInput.Text, FilterKind.DisplayName, null);
                }
            }
            catch { }
        }

        // Handles user selection in the contacts list
        // Calls the OnCompleted method with the selected contact as parameter
        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var item = (ContactsList.SelectedItem as ContactWithImage);
                    if (OnCompleted != null && item != null)
                    {
                        OnCompleted(this, new ContactChooserTaskEventArgs() { Contact = item.Contact });
                        NavigationService.GoBack();
                    }
                }
            }
            catch { }
        }

        #endregion

        #region ContactChooserPage helper methods

        // Clears contacts list from display
        private void ClearContacts()
        {
            ContactsList.ItemsSource = null;
        }

        // Returns the group header the contact should be listed under.
        private char GetGroupHeader(Contact contact)
        {
            try
            {
                // Sorting is by first name is available or the display name.
                string name = (contact.CompleteName != null) ? contact.CompleteName.FirstName : contact.DisplayName;
                char l = name.ToLower()[0];
                if (l >= 'a' && l <= 'z')
                {
                    return l;
                }
            }
            catch { }
            return '#';
        }

        // Hide progress indicator
        private void HideProgress()
        {
            ProgressIndicator.IsIndeterminate = false;
            ProgressIndicator.IsVisible = false;
        }

        // Creates a list of empty groups to which contacts from the search results will be added
        private void InitGroups()
        {
            try
            {
                emptyGroups = alphabet
                    .Select(c => new ContactsGroup<ContactWithImage>(c.ToString(), new List<ContactWithImage>()))
                    .ToList();
            }
            catch { }
        }

        // Loads contacts from the device
        internal void LoadContacts()
        {
            try
            {
                // Show progress
                ShowProgress();

                // Search
                contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(OnContactsSearchCompleted);
                contacts.SearchAsync("", FilterKind.None, null);
            }
            catch { }
        }

        // Show progress indicator
        private void ShowProgress()
        {
            ProgressIndicator.IsIndeterminate = true;
            ProgressIndicator.IsVisible = true;
        }

        #endregion
    }
}