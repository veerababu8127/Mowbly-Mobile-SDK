using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace CloudPact.MowblyFramework.Core.Utils.ImageGallery
{
    class ImageGalleryViewer : IDisposable
    {
        bool isDisposed = false;
        PhoneApplicationFrame frame;
        PhoneApplicationPage callerPage;
        public object galleryData;
        
        public ImageGalleryViewer(object galleryDataSent)
        {
            this.galleryData = galleryDataSent;
        }

        public void launchGallery()
        {
            
            frame = Application.Current.RootVisual as PhoneApplicationFrame;
            callerPage = frame.Content as PhoneApplicationPage;

            frame.Navigated += OnNavigated;
            PhoneApplicationService.Current.State["param"] = galleryData;
            Uri uri = new System.Uri("/CloudPact.MowblyFramework;component/Core/Utils/ImageGallery/ImageGalleryViewer.xaml", UriKind.Relative);
            frame.Navigate(uri);
        }
        
        #region Event handlers

        // Event handler for navigation
        void OnNavigated(object sender, NavigationEventArgs e)
        {
            try
            {
                Mowbly.AppNavigatedToInternalPage = true;
                if (e.Content == callerPage)
                {
                    // Navigation to caller page. Dispose off stuff.
                    Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to view page. Reason: " + ex.Message);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    
                }
            }
            catch { }
        }

        #endregion
    }
}
