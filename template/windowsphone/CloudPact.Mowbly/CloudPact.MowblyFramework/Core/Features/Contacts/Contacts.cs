//-----------------------------------------------------------------------------------------
// <copyright file="Contacts.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Ui;
using CloudPact.MowblyFramework.Core.Utils;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Contacts : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_PREFERENCES; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            string callbackId = message.CallbackId;
            List<string> properties = null;
                        
            try
            {
                switch (message.Method)
                {
                    case "callContact":

                        Logger.Info("Calling contact...");

                        // Get the phone number
                        string phonenumber = message.Args[0] as string;

                        // Create and show the PhoneCall task
                        PhoneCallTask phoneCallTask = new PhoneCallTask();
                        phoneCallTask.PhoneNumber = phonenumber;
                        UiDispatcher.BeginInvoke(() =>
                            {
                                phoneCallTask.Show();
                            });

                        // Set app navigated to external page
                        Mowbly.AppNavigatedToExternalPage = true;
                        break;

                    case "deleteContact":

                        Logger.Info("Deleting contact...");

                        string contactId = message.Args[0] as string;

                        try
                        {

                            ContactStore contactStore = await ContactStore.CreateOrOpenAsync(
                                ContactStoreSystemAccessMode.ReadWrite,
                                ContactStoreApplicationAccessMode.ReadOnly);
                            await contactStore.DeleteContactAsync(contactId);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            string error = String.Concat(
                                Mowbly.GetString(Constants.STRING_CONTACT_DELETE_ERROR),
                                e.Message);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = MethodResult.FAILURE_CODE,
                                    Error = new MethodError
                                    {
                                        Message = error
                                    }
                                });
                        }
                        break;

                    case "findContact":
                        
                        // Read args
                        string filter = message.Args[0] as string;
                        JToken options = message.Args[1] as JToken;
                        properties = options["properties"].ToObject<List<string>>();
                        double limit = (double)options["limit"];

                        Logger.Info("Searching contacts for " + filter);

                        // Contacts search results handler
                        EventHandler<MowblyContactsSearchEventArgs> OnContactsSearchCompleted = null;
                        OnContactsSearchCompleted = (sender, e) =>
                        {
                            if (e.Status)
                            {
                                // Notify result to JS
                                InvokeCallbackJavascript(message.CallbackId, new MethodResult { Result = e.W3Contacts });
                            }
                            else
                            {
                                InvokeCallbackJavascript(message.CallbackId, new MethodResult { Result = null });
                            }
                        };
                        
                        if (Regex.IsMatch(filter, @"^[0-9()-]+$"))
                        {
                            // Only numbers, search by phone number
                            SearchContactInUserDataAsync(filter, OnContactsSearchCompleted, true, FilterKind.PhoneNumber, limit, properties);
                        }
                        else
                        {
                            // Search by display name
                            SearchContactInUserDataAsync(filter, OnContactsSearchCompleted, true, FilterKind.DisplayName, limit, properties);
                        }
                        break;

                    case "pickContact":

                        
                        properties = ((JToken)message.Args[0]).ToObject<List<string>>();
                        ContactChooserTask contactChooserTask = new ContactChooserTask(callbackId);
                        EventHandler<ContactChooserTaskEventArgs> OnContactChooserTaskCompleted = null;
                        OnContactChooserTaskCompleted = (sender, e) => 
                            {
                                // Unsubscribe
                                contactChooserTask.OnCompleted -= OnContactChooserTaskCompleted;

                                // Notify result to JS
                                if (e.Contact != null)
                                {
                                    W3Contact contact = new W3Contact(e.Contact, properties);
                                    InvokeCallbackJavascript(e.CallbackId, new MethodResult { Result = contact });
                                }
                                else
                                {
                                    InvokeCallbackJavascript(e.CallbackId, new MethodResult
                                        {
                                            Code = MethodResult.FAILURE_CODE,
                                            Error = new MethodError
                                            {
                                                Message = Mowbly.GetString(Constants.STRING_ACTIVITY_CANCELLED)
                                            }
                                        });
                                }
                            };

                        // Subscribe to OnCompleted event
                        contactChooserTask.OnCompleted += OnContactChooserTaskCompleted;

                        // Show contact chooser task
                        UiDispatcher.BeginInvoke(() =>
                            {
                                try
                                {
                                    contactChooserTask.Show();
                                }
                                catch (Exception e)
                                {
                                    // Might fail at times since navigation is not allowed when task is not in foreground
                                    string error = String.Concat(Mowbly.GetString(Constants.STRING_CONTACT_CHOOSE_FAILED), e.Message);
                                    Logger.Error(error);
                                    InvokeCallbackJavascript(callbackId, new MethodResult
                                    {
                                        Code = MethodResult.FAILURE_CODE,
                                        Error = new MethodError
                                        {
                                            Message = error
                                        }
                                    });
                                }
                            });
                        break;

                    case "saveContact":

                        try
                        {
                            // Create W3Contact object from JS contact
                            W3Contact w3Contact = JsonConvert.DeserializeObject<W3Contact>(message.Args[0].ToString());
                            string storedContactId = await w3Contact.SaveToNativeContactStore();
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Result = storedContactId
                                });
                        }
                        catch (Exception e)
                        {
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_CONTACT_SAVE_FAILED), e.Message);
                            Logger.Error(error);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = MethodResult.FAILURE_CODE,
                                    Error = new MethodError
                                    {
                                        Message = error
                                    }
                                });
                        }

                        break;

                    case "viewContact":

                        // Get the param and type
                        string id = message.Args[0] as string;
                        Tuple<string, string> paramAndType = await GetSearchParamAndType(id);
                        if (paramAndType != null)
                        {
                            string param = paramAndType.Item1;
                            string type = paramAndType.Item2;
                            ContactViewerTask contactViewerTask = new ContactViewerTask(param, type);

                            // Show contact viewer task
                            UiDispatcher.BeginInvoke(() =>
                            {
                                try
                                {
                                    contactViewerTask.Show();
                                }
                                catch (Exception e)
                                {
                                    // Might fail at times since navigation is not allowed when task is not in foreground
                                    string error = String.Concat(Mowbly.GetString(Constants.STRING_CONTACT_VIEW_FAILED), e.Message);
                                    Logger.Error(error);
                                    InvokeCallbackJavascript(callbackId, new MethodResult
                                    {
                                        Code = MethodResult.FAILURE_CODE,
                                        Error = new MethodError
                                        {
                                            Message = error
                                        }
                                    });
                                }
                            });
                        }
                        else
                        {
                            string error = String.Concat(Mowbly.GetString(Constants.STRING_CONTACT_VIEW_FAILED),
                                Mowbly.GetString(Constants.STRING_CONTACT_NOT_FOUND));
                            Logger.Error(error);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Error = new MethodError
                                {
                                    Message = error
                                }
                            });
                        }

                        break;

                    default:
                        Logger.Error("Feature " + Name + " does not support method " + message.Method);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception occured. Reason - " + e.Message);
            }
        }

        #endregion

        #region Contacts

        async Task<Tuple<string, string>> GetSearchParamAndType(string id)
        {
            Tuple<string, string> result = null;
            ContactStore contactStore = await ContactStore.CreateOrOpenAsync(
                    ContactStoreSystemAccessMode.ReadOnly,
                    ContactStoreApplicationAccessMode.ReadOnly);
            StoredContact contact = await contactStore.FindContactByIdAsync(id);
            if (contact != null)
            {
                result = await SearchContactInUserDataAsync(contact.DisplayName);
            }

            return result;
        }

        Task<Tuple<string, string>> SearchContactInUserDataAsync(string filter)
        {
            TaskCompletionSource<Tuple<string, string>> tcs =
                new TaskCompletionSource<Tuple<string, string>>();
            Tuple<string, string> result = null;
            EventHandler<MowblyContactsSearchEventArgs> OnContactsSearchCompleted = (sender, e) =>
            {
                if (e.Status)
                {
                    Contact c = e.Contacts.FirstOrDefault();
                    // Param can be Phone or Email or Name
                    ContactPhoneNumber phone = c.PhoneNumbers.FirstOrDefault();
                    if (phone != null)
                    {
                        result = Tuple.Create<string, string>(phone.PhoneNumber, phone.Kind.ToString());
                    }
                    else
                    {
                        ContactEmailAddress email = c.EmailAddresses.FirstOrDefault();
                        if (email != null)
                        {
                            result = Tuple.Create<string, string>(email.EmailAddress, email.Kind.ToString());
                        }
                        else
                        {
                            CompleteName name = c.CompleteName;
                            if (name != null)
                            {
                                result = Tuple.Create<string, string>(name.FirstName, "name");
                            }
                        }
                    }
                }
                tcs.SetResult(result);
            };

            SearchContactInUserDataAsync(filter, OnContactsSearchCompleted);

            return tcs.Task;
        }

        // Searches in Contacts for display name and then the phonenumber/email
        void SearchContactInUserDataAsync(
            string filter,
            EventHandler<MowblyContactsSearchEventArgs> OnSearchCompleted,
            bool shouldIncludeW3ContactsInResults = false,
            FilterKind filterKind = FilterKind.DisplayName,
            double limit = 1,
            List<string> properties = null)
        {
            Microsoft.Phone.UserData.Contacts oContacts = new Microsoft.Phone.UserData.Contacts();

            EventHandler<ContactsSearchEventArgs> OnContactsSearchCompleted = null;
            OnContactsSearchCompleted = (object sender, ContactsSearchEventArgs e) =>
            {
                // If results are empty, try searching with other contact fields
                if (e.Results.Count() == 0)
                {
                    switch (e.FilterKind)
                    {
                        case FilterKind.PhoneNumber:
                            // Phonenumber failed. Search by Display name
                            oContacts.SearchAsync(e.Filter, FilterKind.DisplayName, e.State);
                            break;

                        case FilterKind.DisplayName:
                            // Display name failed. Search by email
                            oContacts.SearchAsync(e.Filter, FilterKind.EmailAddress, e.State);
                            break;

                        default:
                            // No results
                            // Unsubscribe
                            oContacts.SearchCompleted -= OnContactsSearchCompleted;

                            // Notify event handler - Error
                            OnSearchCompleted(this, 
                                new MowblyContactsSearchEventArgs(
                                    Mowbly.GetString(Constants.STRING_CONTACT_NOT_FOUND)));
                            break;
                    }
                }
                else
                {
                    // Unsubscribe
                    oContacts.SearchCompleted -= OnContactsSearchCompleted;

                    MowblyContactsSearchEventArgs eventArgs;
                    
                    // Create W3Contacts - if requested
                    if (shouldIncludeW3ContactsInResults)
                    {
                        int numResults = 0;
                        List<W3Contact> w3Contacts = new List<W3Contact>();
                        foreach (Contact contact in e.Results)
                        {
                            w3Contacts.Add(new W3Contact(contact, properties));
                            numResults++;
                            if (limit == numResults) break;
                        }

                        // Create EventArgs - with W3Contact list
                        eventArgs = new MowblyContactsSearchEventArgs(e.Results, w3Contacts);
                    }
                    else
                    {
                        // Create EventArgs - without W3Contact list
                        eventArgs = new MowblyContactsSearchEventArgs(e.Results);
                    }
                    // Notify event handler - Result
                    OnSearchCompleted(this, eventArgs);
                }
            };
            oContacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(OnContactsSearchCompleted);
            // Start search
            oContacts.SearchAsync(filter, filterKind, null);
        }

        #endregion

        #region MowblyContactsSearchEventArgs class

        class MowblyContactsSearchEventArgs
        {
            internal IEnumerable<Contact> Contacts { get; private set; }

            internal string Error { get; private set; }

            internal bool Status { get; private set; }

            internal List<W3Contact> W3Contacts { get; private set; }

            internal MowblyContactsSearchEventArgs(IEnumerable<Contact> contacts)
            {
                Status = true;
                Contacts = contacts;
            }

            internal MowblyContactsSearchEventArgs(IEnumerable<Contact> contacts, List<W3Contact> w3Contacts)
            {
                Status = true;
                Contacts = contacts;
                W3Contacts = w3Contacts;
            }

            internal MowblyContactsSearchEventArgs(string error)
            {
                Status = false;
                Error = error;
            }
        }

        #endregion
    }
}
