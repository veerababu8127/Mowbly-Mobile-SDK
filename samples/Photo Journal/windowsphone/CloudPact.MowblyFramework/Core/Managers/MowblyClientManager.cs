//-----------------------------------------------------------------------------------------
// <copyright file="MowblyClientManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Controls;
using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Ui;
using CloudPact.MowblyFramework.Core.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace CloudPact.MowblyFramework.Core.Managers
{
    public class MowblyClientManager
    {
        const string KEY_APP_DATA = "__app__data__";

        internal Dictionary<string, object> AppData { get; private set; }

        Progress<TaskProgress> progress =
            new Progress<TaskProgress>();

        /// <summary>
        /// Tells if app woke from tombstone state
        /// </summary>
        internal bool IsAppFromTombstone { get; private set; }

        /// <summary>
        /// Event raised whhen app is launched for first time
        /// </summary>
        internal event EventHandler<LaunchingEventArgs> OnAppLaunch;

        /// <summary>
        /// Event raised when app goes to background
        /// </summary>
        internal event TombstoneEventHandler OnAppMoveToBackground;

        /// <summary>
        /// Event raised when app comes to foreground
        /// </summary>
        internal event EventHandler OnAppMoveToForeground;

        /// <summary>
        /// Event raised before app is closed
        /// </summary>
        internal event EventHandler<ClosingEventArgs> OnAppClose;

        /// <summary>
        /// Event raised when device back key is pressed
        /// </summary>
        internal event EventHandler<CancelEventArgs> OnBackKeyPress;

        #region Singleton

        static readonly Lazy<MowblyClientManager> instance =
            new Lazy<MowblyClientManager>(() => new MowblyClientManager());

        MowblyClientManager()
        {
            // Initialize AppData
            AppData = new Dictionary<string, object>();

            // Subscribe to progress changed event to track installation progress
            progress.ProgressChanged += OnProgressChanged;
        }

        ~MowblyClientManager()
        {
            // Unsubscribe from progress changed event
            progress.ProgressChanged -= OnProgressChanged;
        }

        public static MowblyClientManager Instance { get { return instance.Value; } }

        #endregion

        
        #region App launchers

        internal void LaunchDefaultPage()
        {
            Logger.Debug("Launching default page...");
            LaunchHomePage();
        }

        void LaunchHomePage()
        {
            LaunchPage(Mowbly.GetProperty<string>(Constants.PROPERTY_TITLE_HOME_PAGE),
                Mowbly.GetProperty<string>(Constants.PROPERTY_URL_HOME_PAGE));

            
        }

        void LaunchPage(string pageName, string url)
        {
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                {"retainPageInViewStack", true},
                {"showProgress", true}
            };
            UiDispatcher.BeginInvoke(() =>
            {
                PageManager.Instance.OpenPage(pageName, url, null, options);
            });
        }

        #endregion

        #region App Events

        // Invokes on Application_launching event
        public void AppLaunching(LaunchingEventArgs e)
        {
            // Set the SynchronizationContext TaskScheduler
            Mowbly.UiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            // Raise event - OnAppLaunch
            if(OnAppLaunch != null)
            {
                OnAppLaunch(this, e);
            }
        }

        public bool DoesUAexists()
        {
            var UA = PreferencesManager.Instance.Get<string>("__mowbly_useragent");
            if (UA == null)
            {
                return false;
            }
            return true;
        }

        public void PutUA(string UA)
        {
            PreferencesManager.Instance.Put<string>("__mowbly_useragent", UA);
        }

        public string GetUserAgent()
        {
            string CustomeUserAgent = Mowbly.GetUserAgent();
            string DefaultUserAgent = PreferencesManager.Instance.Get<string>("__mowbly_useragent");
            return CustomeUserAgent + "##" + DefaultUserAgent; 
        }

        // Invokes on Application_Activated event
        public void AppActivated(ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                // Dormant [Background to forground]
                // Raise event - OnAppMoveToForeground
                if (OnAppMoveToForeground != null)
                {
                    OnAppMoveToForeground(this, new EventArgs());
                }
            }
            else
            {
                // Set app from tombstone
                IsAppFromTombstone = true;

                // Set the SynchronizationContext TaskScheduler. 
                // Tombstone would have cleared this
                Mowbly.UiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                // Active from Tombstone state
                if (PhoneApplicationService.Current.State.ContainsKey(KEY_APP_DATA))
                {
                    // Read saved data
                    AppData =
                        PhoneApplicationService.Current.State[KEY_APP_DATA] as Dictionary<string, object>;
                }
            }
        }

        // Invokes on Application_Deactivated event
        public void AppDeactivated(DeactivatedEventArgs e)
        {
            //Run on back ground tasks
            RunBackgroundTasks();
            // Raise event OnAppMoveToBackground
            // Handlers can put their data into the applicationData dictionary
            AppData = new Dictionary<string, object>();
            if (OnAppMoveToBackground != null)
            {
                OnAppMoveToBackground(this, new TombstoneEventArgs(AppData));
            }

            // Store the applicationData dictionary into PhoneApplicationState
            PhoneApplicationService.Current.State[KEY_APP_DATA] = AppData;
        }

        // Invokes when the app is closed
        public void AppClosing(ClosingEventArgs e)
        {
            // Raise event - OnAppClose
            if(OnAppClose != null)
            {
                OnAppClose(this, e);
            }
        }

        // Invoked when device back key pressed
        public void BackKeyPress(CancelEventArgs e)
        {
            // Notify event handlers
            if (OnBackKeyPress != null)
            {
                OnBackKeyPress(null, e);
            }

            // If none has acted, proceed closing the active page
            if (!e.Cancel)
            {
                PageModel activePage = PageManager.Instance.ActivePage;
                if (activePage != null)
                {
                    if (!activePage.Name.Equals(Mowbly.GetProperty<string>(Constants.PROPERTY_TITLE_HOME_PAGE)))
                        {
                            activePage.Close(true);
                            e.Cancel = true;
                        }
                    }
                }
        }

        // Invoked when the app has navigated to Main page
        public void NavigatedToMainPage(IMowblyPhoneApplicationPage phoneApplicationPage, NavigationEventArgs e)
        {
            // Check if the page has resumed from tombstone
            if (phoneApplicationPage.IsNewInstance && e.NavigationMode == NavigationMode.Back)
            {
                // Set app from tombstone. 
                // Redundant setting. Already done in OnAppActivated. Can be used in future.
                IsAppFromTombstone = true;
            }

            // Set active PhoneApplicationPage
            SetActivePhoneApplicationPage(phoneApplicationPage);

            // Check if app had navigated back from external/internal app
            if (!Mowbly.AppNavigatedToExternalPage &&
                !Mowbly.AppNavigatedToInternalPage)
            {
                // Trigger foreground tasks
                RunForegroundTasks();
            }
            else
            {
                Mowbly.AppNavigatedToExternalPage = false;
                Mowbly.AppNavigatedToInternalPage = false;
            }

            if (PageManager.Instance.ActivePage == null)
            {
                LaunchHomePage();
            }
        }
        

        /// <summary>
        /// Sets the active PhoneApplicationPage.
        /// Should be set by the app that uses the framework.
        /// </summary>
        /// <param name="phoneApplicationPage">
        /// <see cref="CloudPact.MowblyFramework.Core.Ui.IMowblyPhoneApplicationPage">IMowblyPhoneApplicationPage</see> object
        /// </param>
        public void SetActivePhoneApplicationPage(IMowblyPhoneApplicationPage phoneApplicationPage)
        {
            Mowbly.ActivePhoneApplicationPage = phoneApplicationPage;
        }

        /// <summary>
        /// Sets the properties resource.
        /// Should be set by the app that uses the framework.
        /// </summary>
        /// <param name="properties">
        /// <see cref="System.Resources.ResourceManager">ResourceManager</see> object
        /// </param>
        public static void SetPropertiesResource(ResourceManager properties)
        {
            Mowbly.Properties = properties;
        }

        /// <summary>
        /// Set the strings resource.
        /// Should be set by the app that uses the framework.
        /// </summary>
        /// <param name="strings">
        /// <see cref="System.Resources.ResourceManager">ResourceManager</see> object
        /// </param>
        public static void SetStringsResource(ResourceManager strings)
        {
            Mowbly.Strings = strings;
        }

        #endregion


             

        #region Update cycle

        void RunForegroundTasks()
        {
            IForegroundTasksManager foregroundTasksMgr =
                    MowblyFramework.Core.Mowbly.ForegroundTasksManager;

            // Start foreground tasks
            foregroundTasksMgr.RunTasksAsync(progress);
        }

        void RunBackgroundTasks()
        {
            IBackgroundTaskManager backgroundTaskMgr =
                MowblyFramework.Core.Mowbly.BackgroundTaskManager;

            backgroundTaskMgr.RunTasksAsync(progress);
        }

        async void OnProgressChanged(object sender, TaskProgress e)
        {
            Logger.Info("[PROGRESS]" + e.Code + " - " + e.Status + " - " + e.Message);

            PageManager pageMgr = PageManager.Instance;
        }

        /// <summary>
        /// Launches the default app page if app did not wake from tombstone
        /// Pagemanager takes care of ui restore on tombstone case
        /// </summary>
        void LaunchDefaultAppPage()
        {
            if (!IsAppFromTombstone)
            {
                LaunchDefaultPage();
            }
        }

        #endregion
    }

    #region Event definitions

    delegate void TombstoneEventHandler(MowblyClientManager sender, TombstoneEventArgs e);

    class TombstoneEventArgs : EventArgs
    {
        Dictionary<string, object> applicationData;

        /// <summary>
        /// Application data dictionary that was saved before application went to tombstone
        /// </summary>
        internal Dictionary<string, object> ApplicationData { get { return applicationData; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public TombstoneEventArgs() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationData">
        /// Application data dictionary that was saved before application went to tombstone
        /// </param>
        public TombstoneEventArgs(Dictionary<string, object> applicationData)
        {
            this.applicationData = applicationData;
        }
    }

    #endregion
}
