//-----------------------------------------------------------------------------------------
// <copyright file="IMowblyPhoneApplicationPage.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System.Windows.Controls;

namespace CloudPact.MowblyFramework.Core.Ui
{
    public interface IMowblyPhoneApplicationPage
    {
        /// <summary>
        /// Tells if the page instance is new or resumed
        /// </summary>
        bool IsNewInstance { get; }

        /// <summary>
        /// Returns the root content panel of the PhoneApplicationPage
        /// </summary>
        /// <returns>
        /// <see cref="System.Windows.Controls.Grid">Grid</see> object
        /// </returns>
        Grid GetContentPanel();

        /// <summary>
        /// Canvas containing a <see cref="System.Media.VideoBrush">VideoBrush</see> object
        ///  to work with camera feature
        /// </summary>
        /// <returns>
        /// <see cref="System.Windows.Controls.Canvas">Canvas</see> object
        /// </returns>
        Canvas GetViewFinderCanvas();

        /// <summary>
        /// Hide progress bar
        /// </summary>
        void HideProgress();

        /// <summary>
        /// Show progress bar with the specified text
        /// </summary>
        /// <param name="message"></param>
        void ShowProgress(string message);
    }
}
