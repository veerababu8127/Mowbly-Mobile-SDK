//-----------------------------------------------------------------------------------------
// <copyright file="ContactsGroup.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
// <reference>http://lukencode.com/2011/12/01/windows-phone-mango-contact-chooser-task/</reference>
//-----------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class ContactsGroup<T> : List<T>
    {
        /// <summary>
        /// Title of the contact group
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title of the contact group</param>
        /// <param name="items">List of contact items</param>
        public ContactsGroup(string title, IEnumerable<T> items)
            :base(items)
        {
            this.Title = title;            
        }

        public override bool Equals(object obj)
        {
            ContactsGroup<T> that = obj as ContactsGroup<T>;
            return (that != null) && (this.Title.Equals(that.Title));
        }

        public override int GetHashCode()
        {
            return this.Title.GetHashCode();
        }
    }
}
