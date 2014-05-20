//-----------------------------------------------------------------------------------------
// <copyright file="PageManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Features;
using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Ui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CloudPact.MowblyFramework.Core.Managers
{
    /// <summary>
    /// Direction that current view is navigating to in the view stack
    /// </summary>
    public enum NavigationDirection
    {
        Forward = 0,
        Backward
    };

    /// <summary>
    /// Direction that applies to the next immediate animation
    /// </summary>
    public enum AnimationDirection
    {
        Forward = 0,
        Backward
    };

    class PageManager
    {
        const string KEY_PAGES = "pages";

        Stack<PageModel> pages =  new Stack<PageModel>();

        AnimationDirection animationDirection;

        PageModel activePage;

        NavigationDirection navigationDirection;

        /// <summary>
        /// Current active page
        /// </summary>
        internal PageModel ActivePage { get { return activePage; } }

        #region Singleton

        static readonly Lazy<PageManager> instance =
            new Lazy<PageManager>(() => new PageManager());

        PageManager() 
        {
            // Subscribe to MowblyClientManager client manager app events
            MowblyClientManager.Instance.OnAppMoveToBackground += OnAppMoveToBackground;
            MowblyClientManager.Instance.OnAppMoveToForeground += OnAppMoveToForeground;

            // Subscribe to Network events
            NetworkManager.Instance.OnNetworkConnected += OnNetworkConnected;
            NetworkManager.Instance.OnNetworkDisconnected += OnNetworkDisconnected;            

            // Handle app from tombstone
            if (MowblyClientManager.Instance.IsAppFromTombstone)
            {
                HandleAppFromTombstoneAsync();
            }
        }

        ~PageManager()
        {
            // Subscribe to MowblyClientManager client manager app events
            MowblyClientManager.Instance.OnAppMoveToBackground -= OnAppMoveToBackground;
            MowblyClientManager.Instance.OnAppMoveToForeground -= OnAppMoveToForeground;

            // Unsubscribe from Network events
            NetworkManager.Instance.OnNetworkConnected -= OnNetworkConnected;
            NetworkManager.Instance.OnNetworkDisconnected -= OnNetworkDisconnected;           
        }

        internal static PageManager Instance { get { return instance.Value; } }

        #endregion

        #region Page stack management (Public)

        internal void CloseAllPages()
        {
            // Remove active page from view
            RemovePageModelFromView(activePage);

            // Dispose pages
            foreach(PageModel page in pages)
            {
                page.Dispose();
            }

            // Remove all pages from stack
            pages.Clear();
        }

        internal void ClosePage()
        {
            // Pop the active page
            PopPageModel();
            // Pop till a retained in stack page model
            foreach (var pageModel in pages)
            {
                if (pageModel.RetainInStack)
                {
                    AddPageModelToView(pageModel);
                    activePage = pageModel;
                    break;
                }
                else
                {
                    pages.Pop();
                    pageModel.Dispose();
                }
            }
            // Resume active page
            activePage.Resume(false);
        }


        


        internal void OpenPage(string name, string url, string data, Dictionary<string, object> options)
        {
            // Check if the page is in the stack
            int index = -1;
            if (pages.Count > 0 && IsPageInStack(name, out index))
            {
                // Page model found in the stack
                // Set navigation direction
                navigationDirection = NavigationDirection.Backward;

                // Pop page model till the found page model
                do
                {
                    PopPageModel();
                    index--;
                } while (index > 0);

                // Set pagemodel resumed
                activePage.Resume(true, options, data);

                // Add the active page model to the view
                AddPageModelToView(activePage);
            }
            else
            {
                // New page model
                // Set navigation direction
                navigationDirection = NavigationDirection.Forward;

                // Remove the active page model
                if (activePage != null)
                {
                    if (activePage.RetainInStack)
                    {
                        // Remove from view only
                        RemovePageModelFromView(activePage);
                    }
                    else
                    {
                        // Pop out and dispose
                        PopPageModel();
                    }
                }

                // Create Page model and push
                PushPageModel(new PageModel
                {
                    Name = name,
                    Url = url,
                    Data = data,
                    Parent = ((Func<string>)(() =>
                    {
                        PageModel parentPageModel = GetParentPageModel();
                        return (parentPageModel == null) ? "" : parentPageModel.Name;
                    }))(),
                    ShowProgress = (bool)options["showProgress"],
                    RetainInStack = (bool)options["retainPageInViewStack"]
                });
            }
        }

        /// <summary>
        /// Gets the <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> 
        ///  object corresponding to the specified name in the page stack.
        /// </summary>
        /// <param name="pageName">Name of the 
        /// <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> object
        /// </param>
        /// <returns></returns>
        internal PageModel GetPageModelByName(string pageName)
        {
            return pages.Where(page => page.Name == pageName).FirstOrDefault();
        }

        internal void Reset()
        {
            UiDispatcher.BeginInvoke(() =>
            {
                // Close all pages
                CloseAllPages();

                // Clear active page
                activePage = null;
            });
        }

        #endregion

        #region Page stack management (Private)
        
        void AddPageModelToView(PageModel pageModel)
        {
            // Add the browser to content panel of active page
            Mowbly.ActivePhoneApplicationPage.GetContentPanel().Children.Add(
                pageModel.Browser);
        }

        PageModel GetParentPageModel()
        {
            // Returns the last active page on stack
            // Should be retained in view stack
            PageModel pageModel = null;
            foreach (var pm in pages)
            {
                if (pm.RetainInStack)
                {
                    pageModel = pm;
                    break;
                }
            }

            return pageModel;
        }

        bool IsPageInStack(string name, out int pageIndex)
        {
            int index = -1;
            if (pages.Count > 0)
            {
                index = (from page in pages
                         select page.Name).ToList().IndexOf(name);
            }
            pageIndex = index;
            return index > -1;
        }

        void PopPageModel()
        {
            // Check for the stack pages count and pop
            if(pages.Count > 0)
            {
                try
                {
                    // Remove page model from view
                    RemovePageModelFromView(activePage);

                    // Dispose page model
                    activePage.Dispose();

                    // Pop next page in stack
                    pages.Pop();

                    // Set active page
                    activePage = pages.Peek();
                }
                catch (Exception e)
                {
                    Logger.Error("Error popping page. Reason - " + e.Message);
                }
            }
        }

        void PushPageModel(PageModel pageModel, bool shouldAddToView = true)
        {
            // Load the content in browser - UI thread
            pageModel.LoadContent();

            // Add the page to stack and set as active
            pages.Push(pageModel);
            activePage = pageModel;

            // Add page model to view
            if (shouldAddToView)
            {
                AddPageModelToView(pageModel);
            }

            
        }

        void RemovePageModelFromView(PageModel pageModel)
        {
            // Remove the browser from the active application page
            Grid contentPanel = Mowbly.ActivePhoneApplicationPage.GetContentPanel();
            if (contentPanel.Children.Contains(pageModel.Browser))
            {
                contentPanel.Children.Remove(pageModel.Browser);
            }
        }

        #endregion

        #region Tombstone handling

        // Invoked when app wakes from tombstone state
        async void HandleAppFromTombstoneAsync()
        {
            await Task.Run(() =>
            {
                Dictionary<string, object> appData = MowblyClientManager.Instance.AppData;

                // Read the pages from applicationdata and load, if available
                if (appData.ContainsKey(KEY_PAGES))
                {
                    UiDispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            // Get the pages
                            Stack<PageModel> pageStack = JsonConvert.DeserializeObject<Stack<PageModel>>(
                                (string)appData[KEY_PAGES]);

                            // Restore the pages in UI
                            foreach (PageModel page in pageStack)
                            {
                                PushPageModel(page, false);
                            }

                            // Add the activepage to view
                            AddPageModelToView(activePage);
                        }
                        catch (Exception e)
                        {
                            Logger.Debug("Restore pages from tombstone failed. Reason - " + e.Message);
                            Logger.Debug("Lauching default page...");
                            MowblyClientManager.Instance.LaunchDefaultPage();
                        }
                    });
                }
            });
        }

        #endregion

        #region Event handlers

        // Invoked when the app moves to background
        void OnAppMoveToBackground(MowblyClientManager sender, TombstoneEventArgs e)
        {
            // Fire OnBackground event on active page
            if (activePage != null)
            {
                activePage.InvokeJavascript("__mowbly__", "_onBackground");
            }

            // Set the pages data in applicationdata
            e.ApplicationData.Add(KEY_PAGES, JsonConvert.SerializeObject(pages));
        }

        // Invoked when the app resumes to foreground
        void OnAppMoveToForeground(object sender, EventArgs e)
        {
            // Fire OnForeground event on active page
            if (activePage != null)
            {
                activePage.InvokeJavascript("__mowbly__", "_onForeground");
            }
        }

        // Invoked when device connects to a network
        void OnNetworkConnected(object sender, EventArgs e)
        {
            if (activePage != null)
            {
                Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net =
                    Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
                MethodResult r = new MethodResult
                {
                    Code = (int)NetworkManager.Instance.GetActiveNetwork(),
                    Result = net.ToString()
                };
                activePage.InvokeJavascript("__mowbly__.Network", "onConnect", r);
            }
        }

        // Invoked when device disconnects from an active network
        void OnNetworkDisconnected(object sender, EventArgs e)
        {
            if (activePage != null)
            {
                activePage.InvokeJavascript("__mowbly__.Network", "onDisconnect");
            }
        }

        

        #endregion

        #region Utils

        /// <summary>
        /// Invokes the specified javascript message on all pages
        /// </summary>
        /// <param name="message">Javascript message</param>
        internal async Task BroadcastMessageAsync(string message)
        {
            foreach (PageModel page in pages)
            {
                if (page != activePage)
                {
                    await Task.Run(() =>
                    {
                        UiDispatcher.BeginInvoke(() =>
                        {
                            page.InvokeJavascript("__mowbly__", "_onPageMessage", new object[] { message, activePage.Name });
                        });
                    });
                    
                }
            }
        }

        /// <summary>
        /// Posts a javascript message to the specified page
        /// </summary>
        /// <param name="pageName">Name of the page to which message will be sent</param>
        /// <param name="message">Message to send</param>
        internal async Task PostMessageToPageAsync(string pageName, string message)
        {
            PageModel page = pages.Where(p => p.Name == pageName).FirstOrDefault();
            if (page != null)
            {
                await Task.Run(() =>
                {
                    UiDispatcher.BeginInvoke(() =>
                    {
                        page.InvokeJavascript("__mowbly__", "_onPageMessage", new object[] { message, activePage.Name });
                    });
                });
            }
        }

        #endregion
    }
}