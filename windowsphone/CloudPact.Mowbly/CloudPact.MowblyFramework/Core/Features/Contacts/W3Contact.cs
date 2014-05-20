//-----------------------------------------------------------------------------------------
// <copyright file="W3Contact.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using Microsoft.Phone.UserData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

namespace CloudPact.MowblyFramework.Core.Features
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3Contact
    {
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_NICK_NAME = "nickname";
        private const string KEY_ADDRESSES = "addresses";
        private const string KEY_EMAILS = "emails";
        private const string KEY_BIRTHDAY = "birthday";
        private const string KEY_NOTE = "note";
        private const string KEY_ORGANIZATION = "organization";
        private const string KEY_PHONES = "phones";
        private const string KEY_PHOTOS = "photos";
        private const string KEY_URLS = "urls";

        #region Serialization

        [JsonProperty(
            PropertyName = KEY_ID, 
            Required = Required.Default, 
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(
            PropertyName = KEY_NAME, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public W3ContactName Name { get; set; }

        [JsonProperty(
            PropertyName = KEY_NICK_NAME, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public string NickName { get; set; }

        [JsonProperty(
            PropertyName = KEY_ADDRESSES, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<W3ContactAddress> Addresses { get; set; }

        [JsonProperty(
            PropertyName = KEY_EMAILS, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<W3ContactEmail> Emails { get; set; }

        [JsonProperty(
            PropertyName = KEY_BIRTHDAY,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public string BirthDay { get; set; }

        [JsonProperty(
            PropertyName = KEY_NOTE, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Notes { get; set; }

        [JsonProperty(
            PropertyName = KEY_ORGANIZATION, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public W3ContactOrganization Organization { get; set; }

        [JsonProperty(
            PropertyName = KEY_PHONES,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<W3ContactPhone> Phones { get; set; }

        [JsonProperty(
            PropertyName = KEY_PHOTOS,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<W3ContactPhoto> Photos { get; set; }

        [JsonProperty(
            PropertyName = KEY_URLS,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<W3ContactUrl> Urls { get; set; }

        public W3ContactNickName W3ContactNickName { get; set; }

        public W3ContactBirthday W3ContactBirthday { get; set; }

        public W3ContactNotes W3ContactNotes { get; set; }

        private List<string> Properties
        {
            get
            {
                return new List<string>
                {
                    KEY_ID,
                    KEY_ADDRESSES,
                    KEY_BIRTHDAY,
                    KEY_EMAILS,
                    KEY_NAME,
                    KEY_NICK_NAME,
                    KEY_NOTE,
                    KEY_ORGANIZATION,
                    KEY_PHONES,
                    KEY_PHOTOS,
                    KEY_URLS
                };
            }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3Contact() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contact">
        /// <see cref="Microsoft.Phone.UserData.Contact">Contact</see> object
        /// </param>
        /// <param name="properties">List of properties to read from native contact</param>
        public W3Contact(Contact contact, List<string> properties)
        {
            if (properties == null || properties.Count == 0)
            {
                // Default properties
                properties = Properties;
            }

            // id

            // Address
            if (properties.Any(KEY_ADDRESSES.Equals))
            {
                if (contact.Addresses.Count() > 0)
                {
                    Addresses = contact.Addresses.Select(address => new W3ContactAddress(address))
                        .ToList<W3ContactAddress>();
                }
            }

            // Birthday
            if (properties.Any(KEY_BIRTHDAY.Equals))
            {
                if (contact.Birthdays.Count() > 0)
                {
                    W3ContactBirthday = new W3ContactBirthday(contact.Birthdays
                        .Where(birthday => birthday != new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .Select(birthday => birthday.ToString()).FirstOrDefault());
                }
            }

            // Emails
            if (properties.Any(KEY_EMAILS.Equals))
            {
                if (contact.EmailAddresses.Count() > 0)
                {
                    Emails = contact.EmailAddresses.Select(address => new W3ContactEmail(address))
                        .ToList<W3ContactEmail>();
                }
            }

            // Impps - Not supported

            // Name
            if (properties.Any(KEY_NAME.Equals))
            {
                Name = new W3ContactName(contact.CompleteName);
            }

            // Nickname
            if (properties.Any(KEY_NICK_NAME.Equals))
            {
                W3ContactNickName = new W3ContactNickName(contact.CompleteName.Nickname);
            }

            // Note
            if (properties.Any(KEY_NOTE.Equals))
            {
                if (contact.Notes.Count() > 0)
                {
                    W3ContactNotes = new W3ContactNotes(
                        contact.Notes.Aggregate((current, next) => current + Environment.NewLine + next));
                }
            }

            // Organizations
            if (properties.Any(KEY_ORGANIZATION.Equals))
            {
                if (contact.Companies.Count() > 0)
                {
                    Organization = new W3ContactOrganization(contact.Companies.First());
                }
            }

            // Phones
            if (properties.Any(KEY_PHONES.Equals))
            {
                if (contact.PhoneNumbers.Count() > 0)
                {
                    Phones = contact.PhoneNumbers.Select(phone => new W3ContactPhone(phone))
                        .ToList<W3ContactPhone>();
                }
            }

            // Urls
            if (properties.Any(KEY_URLS.Equals))
            {
                if (contact.Websites.Count() > 0)
                {
                    Urls = contact.Websites.Select(website => new W3ContactUrl(W3ContactTypeValueField.KEY_HOME, website))
                        .ToList<W3ContactUrl>();
                }
            }

            // Photos - Get the picture and write it to cache
            if (properties.Any(KEY_PHOTOS.Equals))
            {
                Stream pictureStream = contact.GetPicture();
                FilePath filePath = new FilePath
                {
                    Path = "photo" + new Random().Next() + ".jpg",
                    Level = FileLevel.App,
                    StorageType = StorageType.Cache
                };
                Photos = new List<W3ContactPhoto>
                {
                    new W3ContactPhoto(W3ContactTypeValueField.KEY_HOME, filePath)
                };
                Task.Run(() =>
                {
                    try
                    {
                        FileManager.WriteDataToFile(
                            FileManager.GetAbsolutePath(filePath), pictureStream, false);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error writing contact picture to tmp folder. Reason - " + e.Message);
                    }
                });
            }
        }

        private async Task<StoredContact> ToStoredContact(StoredContact contact)
        {
            if (contact == null)
            {
                ContactStore contactStore = await ContactStore.CreateOrOpenAsync(
                    ContactStoreSystemAccessMode.ReadWrite,
                    ContactStoreApplicationAccessMode.ReadOnly);
                contact = new StoredContact(contactStore);
            }

            IDictionary<string, object> properties = await contact.GetPropertiesAsync();
           
            // Address
            if (Addresses != null && Addresses.Count > 0)
            {
                Addresses.ForEach(address => address.AsStoredContactProperties(ref properties));
            }

            // Birthday
            if (W3ContactBirthday != null)
            {
                W3ContactBirthday.AsStoredContactProperties(ref properties);
            }
            
            // Emails
            if (Emails != null && Emails.Count > 0)
            {
                Emails.ForEach(email => email.AsStoredContactProperties(ref properties));
            }

            // Name
            if (Name != null)
            {
                Name.AsStoredContactProperties(ref properties);
            }

            // NickName
            if (W3ContactNickName != null)
            {
                W3ContactNickName.AsStoredContactProperties(ref properties);
            }

            // Note
            if (W3ContactNotes != null)
            {
                W3ContactNotes.AsStoredContactProperties(ref properties);
            }
            
            // Organization
            if (Organization != null)
            {
                Organization.AsStoredContactProperties(ref properties);
            }

            // Phones
            if (Phones != null && Phones.Count > 0)
            {
                Phones.ForEach(phone => phone.AsStoredContactProperties(ref properties));
            }

            // Photos
            if (Photos != null && Photos.Count > 0)
            {
                W3ContactPhoto firstPhoto = Photos.FirstOrDefault();
                if (firstPhoto != null)
                {
                    await firstPhoto.SetPictureOnStoredContactAsync(contact);
                }
            }

            // Urls
            if (Urls != null && Urls.Count > 0)
            {
                Urls.ForEach(url => url.AsStoredContactProperties(ref properties));
            }

            return contact;
        }

        /// <summary>
        /// Saves the contact to native store
        /// </summary>
        /// <returns>Unique contact id of the stored contact</returns>
        public async Task<string> SaveToNativeContactStore()
        {
            ContactStore contactStore = await ContactStore.CreateOrOpenAsync(
                    ContactStoreSystemAccessMode.ReadWrite, 
                    ContactStoreApplicationAccessMode.ReadOnly);
            StoredContact contact = null;

            // Update contact if already exists
            if (!String.IsNullOrEmpty(Id))
            {
                contact = await contactStore.FindContactByIdAsync(Id);
            }
            if (contact == null)
            {
                contact = new StoredContact(contactStore);
            }

            // Set properties on stored contact
            await ToStoredContact(contact);

            // Save the contact
            await contact.SaveAsync();

            return contact.Id;
        }

        [OnSerializing()]
        public void OnSerializing(StreamingContext context)
        {
            // Update string from W3ContactValueFields
            if (W3ContactNickName != null)
            {
                NickName = W3ContactNickName.Value;
            }
            if (W3ContactBirthday != null)
            {
                BirthDay = W3ContactBirthday.Value;
            }
            if (W3ContactNotes != null)
            {
                Notes = W3ContactNotes.Value;
            }
        }

        [OnDeserialized()]
        public void OnDeserialized(StreamingContext context)
        {
            // Create W3ContactValueFields
            if (NickName != null)
            {
                W3ContactNickName = new W3ContactNickName(NickName);
            }
            if (BirthDay != null)
            {
                W3ContactBirthday = new W3ContactBirthday(BirthDay);
            }
            if (Notes != null)
            {
                W3ContactNotes = new W3ContactNotes(Notes);
            }
        }
    }

    #region W3Contact properties

    /// <summary>
    /// IW3ContactField
    /// </summary>
    public interface IW3ContactField
    {
        /// <summary>
        /// Return key value pair of the contact field properties as StoredContact
        /// <see cref="Windows.Phone.PersonalInformation.KnownContactProperties">KnownContactProperties</see>
        /// </summary>
        void AsStoredContactProperties(ref IDictionary<string, object> properties);

        /// <summary>
        /// Returns the W3Contact field type for the specified KnownContactPropertieskind
        /// </summary>
        /// <param name="knownContactPropertiesKind">KnownContactPropertieskind string</param>
        /// <returns>W3Contact field type</returns>
        string GetW3ContactFieldType<T>(T knownContactPropertiesKind);

        /// <summary>
        /// Returns the KnownContactPropertiesKind for the specified W3Contact field type
        /// </summary>
        /// <param name="w3ContactFieldType">W3Contact field type</param>
        /// <returns>String value of 
        /// <see cref="Windows.Phone.PersonalInformation.KnownContactProperties">KnownContactProperties</see> object
        ///  representing the specified W3Contact field type
        /// </returns>
        string GetKnownContactPropertiesKind(string w3ContactFieldType);
    }

    /// <summary>
    /// Base W3ContactField. Implements IW3ContactField
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class W3ContactField : IW3ContactField
    {
        internal const string KEY_HOME = "home";
        internal const string KEY_WORK = "work";
        internal const string KEY_OTHER = "other";

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactField() { }

        public virtual void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {   
        }

        public virtual string GetW3ContactFieldType<T>(T knownContactPropertiesKind)
        {
            return KEY_WORK;
        }

        public virtual string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            return String.Empty;
        }

        public void SetStoredContactProperty(string property, object value, 
            ref IDictionary<string, object> properties)
        {
            if (properties.ContainsKey(property))
            {
                properties.Remove(property);
            }
            properties.Add(property, value);
        }
    }

    /// <summary>
    /// Generic contact field that contains only string value.
    /// Not serializable. Will be handled in OnSerialized adn OnDeserialized events of W3Contact
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class W3ContactValueField : W3ContactField
    {
        /// <summary>
        /// Value of the contact field
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactValueField() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">String value of the contact field</param>
        public W3ContactValueField(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Generic contact field with type and value
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class W3ContactTypeValueField : W3ContactField
    {
        internal const string KEY_TYPE = "type";
        internal const string KEY_VALUE = "value";

        [JsonProperty(
            PropertyName = KEY_TYPE,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Kind { get; set; }

        [JsonProperty(
            PropertyName = KEY_VALUE,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public object Value { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactTypeValueField() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the contact field</param>
        /// <param name="value">Value of the contact field</param>
        public W3ContactTypeValueField(string type, object value)
        {
            Kind = type;
            Value = value;
        }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            SetStoredContactProperty(GetKnownContactPropertiesKind(Kind), Value, ref properties);
        }
    }

    /// <summary>
    /// Contact name
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactName : W3ContactField
    {
        private const string KEY_FIRST_NAME = "firstName";
        private const string KEY_MIDDLE_NAME = "middleName";
        private const string KEY_LAST_NAME = "lastName";
        private const string KEY_PREFIX = "prefix";
        private const string KEY_SUFFIX = "suffix";

        [JsonProperty(
            PropertyName = KEY_FIRST_NAME, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string FirstName { get; set; }

        [JsonProperty(
            PropertyName = KEY_MIDDLE_NAME, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string MiddleName { get; set; }

        [JsonProperty(
            PropertyName = KEY_LAST_NAME, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string LastName { get; set; }

        [JsonProperty(
            PropertyName = KEY_PREFIX, 
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Prefix { get; set; }

        [JsonProperty(
            PropertyName = KEY_SUFFIX,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Suffix { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactName() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="completeName">
        /// <see cref="Microsoft.Phone.UserData.CompleteName">CompleteName</see> object
        /// </param>
        public W3ContactName(CompleteName completeName)
        {
            this.FirstName = completeName.FirstName;
            this.MiddleName = completeName.MiddleName;
            this.LastName = completeName.LastName;
            this.Prefix = completeName.Title;
            this.Suffix = completeName.Suffix;
        }

        /// <summary>
        /// Sets the name properties on the Stored contact
        /// </summary>
        /// <param name="contact">
        /// <see cref="Windows.Phone.PersonalInformation.StoredContact">StoredContact</see> object
        /// </param>
        public void SetPropertiesOnStoredContact(ref StoredContact contact)
        {
            contact.GivenName = FirstName;
            contact.FamilyName = LastName;
            contact.HonorificPrefix = Prefix;
            contact.HonorificSuffix = Suffix;
        }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            SetStoredContactProperty(KnownContactProperties.FamilyName, LastName, ref properties);
            SetStoredContactProperty(KnownContactProperties.AdditionalName, MiddleName, ref properties);
            SetStoredContactProperty(KnownContactProperties.GivenName, FirstName, ref properties);
            SetStoredContactProperty(KnownContactProperties.DisplayName, 
                String.Format("{0} {1} {2}", (FirstName ?? ""), (MiddleName ?? ""), (LastName ?? "")), 
                ref properties);
            SetStoredContactProperty(KnownContactProperties.HonorificPrefix, Prefix, ref properties);
            SetStoredContactProperty(KnownContactProperties.HonorificSuffix, Suffix, ref properties);
        }
    }

    /// <summary>
    /// Contact Address
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactAddress : W3ContactField
    {
        private const string KEY_STREET = "street";
        private const string KEY_CITY = "city";
        private const string KEY_REGION = "region";
        private const string KEY_COUNTRY = "country";
        private const string KEY_POSTAL_CODE = "postalcode";
        private const string KEY_TYPE = "type";

        [JsonProperty(
            PropertyName = KEY_STREET,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Street { get; set; }

        [JsonProperty(
            PropertyName = KEY_CITY,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string City { get; set; }

        [JsonProperty(
            PropertyName = KEY_REGION,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Region { get; set; }

        [JsonProperty(
            PropertyName = KEY_COUNTRY,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Country { get; set; }

        [JsonProperty(
            PropertyName = KEY_POSTAL_CODE,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string PostalCode { get; set; }

        [JsonProperty(
            PropertyName = KEY_TYPE,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Kind { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactAddress() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">
        /// <see cref="Microsoft.Phone.UserData.ContactAddress">ContactAddress</see> object
        /// </param>
        public W3ContactAddress(Microsoft.Phone.UserData.ContactAddress address)
        {
            CivicAddress civicAddress = address.PhysicalAddress;
            Street = civicAddress.AddressLine1;
            City = civicAddress.City;
            Region = civicAddress.StateProvince;
            Country = civicAddress.CountryRegion;
            PostalCode = civicAddress.PostalCode;
            Kind = address.Kind.ToString().ToLower();
        }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {   
            Windows.Phone.PersonalInformation.ContactAddress address = 
                new Windows.Phone.PersonalInformation.ContactAddress();
            address.StreetAddress = Street;
            address.Locality = City;
            address.Region = Region;
            address.Country = Country;
            address.PostalCode = PostalCode;

            SetStoredContactProperty(GetKnownContactPropertiesKind(Kind), address, ref properties);
        }

        public override string GetW3ContactFieldType<T>(T emailAddressKind)
        {
            string type;
            string key = emailAddressKind.ToString();
            if (key.Equals(KnownContactProperties.Address))
            {
                type = KEY_HOME;
            }
            else if (key.Equals(KnownContactProperties.OtherAddress))
            {
                type = KEY_OTHER;
            }
            else
            {
                type = KEY_WORK;
            }

            return type;
        }

        public override string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            string kind;
            if (w3ContactFieldType.Equals(KEY_HOME))
            {
                kind = KnownContactProperties.Address;
            }
            else if (w3ContactFieldType.Equals(KEY_OTHER))
            {
                kind = KnownContactProperties.OtherAddress;
            }
            else
            {
                kind = KnownContactProperties.WorkAddress;
            }

            return kind;
        }
    }

    /// <summary>
    /// Contact organization
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactOrganization : W3ContactField
    {
        private const string KEY_NAME = "name";
        private const string KEY_JOB_TITLE = "jobTitle";
        private const string KEY_DEPARTMENT = "department";

        [JsonProperty(
            PropertyName = KEY_NAME,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Name { get; set; }

        [JsonProperty(
            PropertyName = KEY_JOB_TITLE,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string JobTitle { get; set; }

        [JsonProperty(
            PropertyName = KEY_DEPARTMENT,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(Constants.STRING_EMPTY)]
        public string Department { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactOrganization() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="companyInfo">
        /// <see cref="Microsoft.Phone.UserData.ContactCompanyInformation">ContactCompanyInformation</see> object
        /// </param>
        public W3ContactOrganization(ContactCompanyInformation companyInfo)
        {
            Name = companyInfo.CompanyName;
            JobTitle = companyInfo.JobTitle;
        }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            SetStoredContactProperty(KnownContactProperties.CompanyName, Name, ref properties);
            SetStoredContactProperty(KnownContactProperties.JobTitle, JobTitle, ref properties);
        }
    }

    /// <summary>
    /// Contact Nickname
    /// </summary>
    public class W3ContactNickName : W3ContactValueField
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactNickName() { }

        public W3ContactNickName(string nickname) : base(nickname) { }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            SetStoredContactProperty(KnownContactProperties.Nickname, Value, ref properties);
        }
    }

    /// <summary>
    /// Contact Birthday
    /// </summary>
    public class W3ContactBirthday : W3ContactValueField
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactBirthday() { }

        public W3ContactBirthday(string birthday) : base(birthday) { }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            long birthdayInMillis;
            if (long.TryParse(Value, out birthdayInMillis))
            {
                SetStoredContactProperty(KnownContactProperties.Birthdate,
                    new DateTimeOffset(new DateTime(1970, 1, 1).AddMilliseconds(birthdayInMillis)), 
                    ref properties);
            }
        }
    }

    /// <summary>
    /// Contact Note
    /// </summary>
    public class W3ContactNotes : W3ContactValueField
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactNotes() { }

        public W3ContactNotes(string notes) : base(notes) { }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
            SetStoredContactProperty(KnownContactProperties.Notes, Value, ref properties);
        }
    }

    /// <summary>
    /// Contact email
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactEmail : W3ContactTypeValueField
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactEmail() { }

        public W3ContactEmail(string type, string value) : base(type, value) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contactEmailAddress">
        /// <see cref="Microsoft.Phone.UserData.ContactEmailAddress">ContactEmailAddress</see> object
        /// </param>
        public W3ContactEmail(ContactEmailAddress contactEmailAddress)
        {
            Kind = GetW3ContactFieldType<EmailAddressKind>(contactEmailAddress.Kind);
            Value = contactEmailAddress.EmailAddress;
        }

        public override string GetW3ContactFieldType<T>(T emailAddressKind)
        {
            string type;
            string key = emailAddressKind.ToString();
            if (key.Equals(KnownContactProperties.Email))
            {
                type = KEY_HOME;
            }
            else if (key.Equals(KnownContactProperties.OtherEmail))
            {
                type = KEY_OTHER;
            }
            else
            {
                type = KEY_WORK;
            }

            return type;
        }

        public override string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            string kind;
            if (w3ContactFieldType.Equals(KEY_HOME))
            {
                kind = KnownContactProperties.Email;
            }
            else if (w3ContactFieldType.Equals(KEY_OTHER))
            {
                kind = KnownContactProperties.OtherEmail;
            }
            else
            {
                kind = KnownContactProperties.WorkEmail;
            }

            return kind;
        }
    }

    /// <summary>
    /// Contact phone
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactPhone : W3ContactTypeValueField
    {
        internal const string KEY_MOBILE = "mobile";
        internal const string KEY_PAGER = "pager";
        internal const string KEY_FAX = "fax";

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactPhone() { }

        public W3ContactPhone(string type, string value) : base(type, value) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contactPhoneNumber">
        /// <see cref="Microsoft.Phone.UserData.ContactPhoneNumber">ContactPhoneNumber</see> object
        /// </param>
        public W3ContactPhone(ContactPhoneNumber contactPhoneNumber)
        {
            Kind = GetW3ContactFieldType<PhoneNumberKind>(contactPhoneNumber.Kind);
            Value = contactPhoneNumber.PhoneNumber;
        }

        public override string GetW3ContactFieldType<T>(T phoneNumberKind)
        {
            string type;
            string key = phoneNumberKind.ToString();
            if (key.Equals(KnownContactProperties.Telephone))
            {
                type = KEY_HOME;
            }
            else if (key.Equals(KnownContactProperties.AlternateTelephone))
            {
                type = KEY_OTHER;
            }
            else if (key.Equals(KnownContactProperties.CompanyTelephone))
            {
                type = KEY_WORK;
            }
            else if (key.Equals(KnownContactProperties.AlternateMobileTelephone))
            {
                type = KEY_PAGER;
            }
            else if (key.Equals(KnownContactProperties.WorkFax))
            {
                type = KEY_FAX;
            }
            else
            {
                type = KEY_MOBILE;
            }

            return type;
        }

        public override string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            string kind;
            if (w3ContactFieldType.Equals(KEY_WORK))
            {
                kind = KnownContactProperties.WorkTelephone;
            }
            else if (w3ContactFieldType.Equals(KEY_HOME))
            {
                kind = KnownContactProperties.Telephone;
            }
            else if (w3ContactFieldType.Equals(KEY_PAGER))
            {
                kind = KnownContactProperties.AlternateMobileTelephone;
            }
            else if (w3ContactFieldType.Equals(KEY_FAX))
            {
                kind = KnownContactProperties.WorkFax;
            }
            else if (w3ContactFieldType.Equals(KEY_OTHER))
            {
                kind = KnownContactProperties.AlternateTelephone;
            }
            else
            {
                kind = KnownContactProperties.MobileTelephone;
            }

            return kind;
        }
    }

    /// <summary>
    /// Contact photo
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactPhoto : W3ContactTypeValueField
    {
        public FilePath PhotoPath { get; set; }

        [OnDeserialized()]
        public void OnDeserialized(StreamingContext context)
        {
            // Set the PhotoPath as Value
            if (Value != null)
            {
                PhotoPath = JsonConvert.DeserializeObject<FilePath>(Value.ToString());
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactPhoto() { }

        public W3ContactPhoto(string type, FilePath value) 
            : base(type, value) 
        {
            PhotoPath = value;
            this.Value = FileManager.GetAbsolutePath(PhotoPath);
        }

        public override void AsStoredContactProperties(ref IDictionary<string, object> properties)
        {
        }

        public override string GetW3ContactFieldType<T>(T phoneNumberKind)
        {
            return String.Empty;
        }

        public override string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            return String.Empty;
        }

        public async Task SetPictureOnStoredContactAsync(StoredContact contact)
        {
            try
            {
                await contact.SetDisplayPictureAsync(FileManager.ReadAsStream(
                    FileManager.GetAbsolutePath(PhotoPath)));
            }
            catch (Exception e)
            {
                Logger.Error("Error setting photo on stored contact. Reason - " + e.Message);
            }
        }
    }

    /// <summary>
    /// Contact url
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class W3ContactUrl : W3ContactTypeValueField
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public W3ContactUrl() { }

        public W3ContactUrl(string type, string value) : base(type, value) { }

        public override string GetW3ContactFieldType<T>(T phoneNumberKind)
        {
            return KEY_WORK;
        }

        public override string GetKnownContactPropertiesKind(string w3ContactFieldType)
        {
            return KnownContactProperties.Url;
        }
    }

    #endregion
}