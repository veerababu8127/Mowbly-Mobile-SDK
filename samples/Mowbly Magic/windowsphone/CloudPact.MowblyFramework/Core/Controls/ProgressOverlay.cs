//-----------------------------------------------------------------------------------------
// <copyright file="ProgressOverlay.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CloudPact.MowblyFramework.Core.Controls
{
    public class ProgressOverlay
    {
        const string progressOverlayTemplate = @"
                <c4f:ProgressOverlay
                    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                    xmlns:slToolkit=""clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit""
                    xmlns:c4f=""clr-namespace:Coding4Fun.Toolkit.Controls;assembly=Coding4Fun.Toolkit.Controls""
                    xmlns:Converters=""clr-namespace:Coding4Fun.Toolkit.Controls.Converters;assembly=Coding4Fun.Toolkit.Controls""
                    Name=""c4fProgressOverlay"">
                    <StackPanel>
                        <TextBlock HorizontalAlignment=""Center"">Loading</TextBlock>
                        <ProgressBar />
                    </StackPanel>
                </c4f:ProgressOverlay>";

        bool isShowing = false;

        Coding4Fun.Toolkit.Controls.ProgressOverlay progressOverlay;

        Grid parent;

        public bool IsShowing { get { return isShowing; } }

        #region Singleton

        static readonly Lazy<ProgressOverlay> instance =
            new Lazy<ProgressOverlay>(() => new ProgressOverlay());

        ProgressOverlay() : base() { }

        public static ProgressOverlay Instance { get { return instance.Value; } }
        
        #endregion

        /// <summary>
        ///  Hides the progress overlay
        ///  Raises OnHide event which can be listened by any page and UI changes done
        /// </summary>
        public void Hide()
        {
            if (progressOverlay != null)
            {
                // Hide and set indeterminate false on progressbar to save battery
                progressOverlay.Hide();
                ((ProgressBar)((StackPanel)progressOverlay.Content).Children[1]).IsIndeterminate = false;

                // Remove from parent
                if (parent != null)
                {
                    UIElementCollection children = parent.Children;
                    if (children.Contains(progressOverlay))
                    {
                        children.Remove(progressOverlay);
                    }
                }
            }
            isShowing = false;
        }

        /// <summary>
        /// Shows the progress overlay
        /// </summary>
        /// <param name="message">Message to display in the progress</param>
        public void Show(string message)
        {
            if (!isShowing)
            {
                if (progressOverlay == null)
                {
                    progressOverlay =
                        XamlReader.Load(progressOverlayTemplate) as Coding4Fun.Toolkit.Controls.ProgressOverlay;
                }

                Grid newParent = Mowbly.ActivePhoneApplicationPage.GetContentPanel();
                // Remove the progress overlay from the current parent if available
                // and if the new parent and the old parent is not same
                if (parent != null && !newParent.Equals(parent))
                {
                    UIElementCollection children = parent.Children;
                    if (children.Contains(progressOverlay))
                    {
                        children.Remove(progressOverlay);
                    }
                }

                // Add it the the new parent
                parent = newParent;
                parent.Children.Add(progressOverlay);
                progressOverlay.Show();
                isShowing = true;
                // Set progressbar indeterminate to true
                ((ProgressBar)((StackPanel)progressOverlay.Content).Children[1]).IsIndeterminate = true;
            }

            // Set the message
            ((TextBlock)((StackPanel)progressOverlay.Content).Children[0]).Text = message;
        }
    }
}
