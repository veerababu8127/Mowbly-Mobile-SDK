//-----------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Features;
using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace CloudPact.MowblyFramework.Core.Ui
{
    [JsonObject(MemberSerialization.OptIn)]
    class PageModel : IDisposable
    {
        const string KEY_HAS_PAGE_RESUMED = "__has_page_resumed__";

        WebBrowser browser;
        
        bool isNavigationFailed;

        bool isNewPageInstance;

        bool isDisposed;

        /// <summary>
        /// Webbrowser object for the page
        /// </summary>
        internal WebBrowser Browser { get{ return browser; } }

        

        /// <summary>
        /// Page context info to send to Mowbly JS layer
        /// </summary>
        internal PageContext PageContext
        {
            get
            {
                return new PageContext
                {
                    Data = this.Data ?? "{}",
                    PageName = this.Name,
                    Orientation = (int)((PhoneApplicationPage)Mowbly.ActivePhoneApplicationPage).Orientation,
                    PageParent = this.Parent,
                    WaitingForResult = false,
                    Preferences = new Dictionary<string, object>(){},                    
                    Network = NetworkInterface.GetIsNetworkAvailable()
                };
            }
        }

        #region Lifecycle

        internal void Resume(bool isResumed, Dictionary<string, object> options = null, 
            string data = Constants.STRING_EMPTY)
        {
            if (isResumed)
            {
                // Resumed from stack
                // Update data and options
                Data = (String.IsNullOrEmpty(data)) ? null : data;
                ShowProgress = (bool)options["showProgress"];
                RetainInStack = (bool)options["retainPageInViewStack"];
            }

            // Raise OnResumed event
            PageEventArgs e = new PageEventArgs(this, new Dictionary<string, object>
                {
                    {KEY_HAS_PAGE_RESUMED, isResumed}
                });
            RaisePageEvent(OnResumed, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the page is closed.
        /// Possible triggers are page close with destroy option true
        /// </summary>
        internal event PageEventHandler OnClosed;

        /// <summary>
        /// Raised when the url content is loaded on the browser
        /// </summary>
        internal event PageEventHandler OnLoaded;

        /// <summary>
        /// Raised when the url content is load on the browser failed
        /// </summary>
        internal event PageEventHandler OnLoadFailed;

        /// <summary>
        /// Raised when the page is resumed.
        /// Possible triggers are returning to view after a child page is closed or 
        /// reopening of the page from page stack
        /// </summary>
        internal event PageEventHandler OnResumed;

        #endregion

        #region Serialization

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Url { get; set; }

        [JsonProperty]
        public string Data { get; set; }

        [JsonProperty]
        public string Parent { get; set; }

        [JsonProperty]
        public bool ShowProgress { get; set; }

        [JsonProperty]
        public bool RetainInStack { get; set; }

        #endregion

        #region Constructor & Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PageModel()
        {
            isNewPageInstance = true;

            // Set default values
            ShowProgress = true;
            RetainInStack = true;

            // Add the browser
            AddBrowser();

            // Subscribe to pagemodel events
            SubscribeToPageModelEvents();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">Url to load in the page</param>
        public PageModel(string url) : this()
        {
            Url = url;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PageModel()
        {
            Dispose();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the page url in the browser
        /// </summary>
        internal void LoadContent()
        {                        
            browser.Navigate(new Uri("Resources" + Url, UriKind.Relative));
        }


        /// <summary>
        /// Sets result to the parent 
        /// <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> object
        /// </summary>
        /// <param name="result">Result string to set</param>
        internal void setResult(string result)
        {
            if (!String.IsNullOrEmpty(Parent))
            {
                PageModel page = PageManager.Instance.GetPageModelByName(Parent);
                if (page != null)
                {
                    page.Data = result;
                }
            }
        }

        /// <summary>
        /// Closes the pagemodel. Possible triggers are device back key press or JS close call.
        /// Fires OnClosed event.
        /// </summary>
        internal void Close(bool isDeviceBackpress)
        {
            RaisePageEvent(OnClosed, new PageEventArgs(this, new Dictionary<string,object>
            {
                {Constants.KEY_IS_DEVICE_BACKPRESS, true}
            }));
        }

        #endregion

        #region Browser

        void AddBrowser()
        {
            browser = new WebBrowser();
            browser.IsScriptEnabled = true;
            browser.ScriptNotify += OnScriptNotify;
            browser.LoadCompleted += OnLoadComplete;
            browser.NavigationFailed += OnNavigationFailed;
        }

        void OnLoadComplete(object sender, NavigationEventArgs e)
        {
            Logger.Debug("Page loading complete");

            // Raise onLoaded event - if new page or navigation failed
            if (isNewPageInstance || isNavigationFailed)
            {
                RaisePageEvent(OnLoaded, new PageEventArgs(this));
                isNewPageInstance = false;
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Logger.Error("Navigation failed for url " + e.Uri.ToString());

            isNavigationFailed = true;

            // Raise onLoadFailed event
            RaisePageEvent(OnLoadFailed, new PageEventArgs());
            try
            {
                // Navigate to page not found
                string pnfUrl = Mowbly.GetProperty<string>(Constants.PROPERTY_URL_PAGE_NOT_FOUND);
                browser.Navigate(new Uri(pnfUrl, UriKind.Relative));
            }
            catch (Exception ex)
            {
                Logger.Error("Error navigating to 404 page. Reason - " + ex.Message);
            }
            e.Handled = true;
        }

        async void OnScriptNotify(object sender, NotifyEventArgs e)
        {
            string message = null;
            JSMessage m;
            try
            {
                message = e.Value;
                Logger.Info("Message from Mowbly: " + message);
                m = JsonConvert.DeserializeObject<JSMessage>(message);
                await FeatureBinder.Instance.InvokeAsync(m);
            }
            catch (Exception ex)
            {
                try
                {
                    if (message.StartsWith("{") && message.EndsWith("}"))
                    {
                        Logger.Error("Failed parsing Mowbly message: " + ex.Message);
                    }
                    else
                    {
                        MessagePrompt prompt = new MessagePrompt();
                        prompt.Title = "Alert";
                        prompt.Message = message;
                        prompt.Show();
                    }
                }
                catch { }
            }
        }

        

        void RemoveBrowser()
        {
            try
            {
                UiDispatcher.BeginInvoke(() =>
                {
                    browser.Source = null;
                    browser.IsScriptEnabled = false;
                    browser.ScriptNotify -= OnScriptNotify;
                    browser.LoadCompleted -= OnLoadComplete;
                    browser.NavigationFailed -= OnNavigationFailed;
                    browser = null;
                });
            }
            catch { }
        }

        #endregion

        #region Javascript bridge

        internal void InvokeJavascript(string jObject, string method, params object[] args)
        {
            string scriptName = "__wp7Plug";
            string options = null;
            try
            {
                WindowsPlug plug = new WindowsPlug();
                plug.JSObject = jObject;
                plug.Method = method;
                plug.Args = args;

                string message = String.Format("Invoking js method -- {0}.{1}({2})", 
                    jObject, 
                    method,
                    JsonConvert.SerializeObject(plug, Formatting.Indented));
                Logger.Info("Message to Mowbly: " + message);

                options = JsonConvert.SerializeObject(plug);
                browser.InvokeScript(scriptName, options);
            }
            catch (Exception e)
            {
                // JS Error for closeApplication. Enable back door to close aplication if
                // user presses it for 3 times.
                if (jObject.Equals("__mowbly__", StringComparison.OrdinalIgnoreCase) &&
	                method.Equals("_onDeviceBackPressed", StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: Implement this
	                /*if (++App.BackKeyPressCount >= 3)
	                {
		                App.CloseApp = true;
	                }*/
                }
                string message = String.Format("Error invoking javascript method {0}.{1}() with parameters {2}. {3}.", 
	                jObject, 
	                method, 
	                args,
	                e.Message);
                Logger.Error(message);
            }
        }

        #endregion

        #region PageModel event handlers

        // Notify pageload to Mowbly JS layer
        void OnPageModelLoaded(PageModel sender, PageEventArgs e)
        {
            InvokeJavascript("__mowbly__", "_onPageLoad", PageContext);
        }

        // Notify pageresume to Mowbly JS layer
        void OnPageModelResumed(PageModel sender, PageEventArgs e)
        {
            Dictionary<string, object> preferences = new Dictionary<string, object>();
            
            string data = String.IsNullOrEmpty(Data) ? "{}" : Data;

            bool hasPageResumed = e.Properties.ContainsKey(KEY_HAS_PAGE_RESUMED) ? 
                (bool)e.Properties[KEY_HAS_PAGE_RESUMED] : true;


            PageContext pageContext = new PageContext
            {
                Data = data,                
                PageName = this.Name,                
                Orientation = (int)((PhoneApplicationPage)Mowbly.ActivePhoneApplicationPage).Orientation,                
                PageParent = this.Parent,
                WaitingForResult = !hasPageResumed,
                Preferences = preferences                
            };

            InvokeJavascript("__mowbly__", "_pageOpened", pageContext);            
        }

        // Notify pageclose to Mowbly JS layer
        void OnPageModelClosed(PageModel sender, PageEventArgs e)
        {
            bool isDeviceBackpress = (bool)e.Properties[Constants.KEY_IS_DEVICE_BACKPRESS];
            if (isDeviceBackpress)
            {
                InvokeJavascript("__mowbly__", "_onDeviceBackPressed");
            }
        }

        #endregion

        #region Utils

        // Subscribe handlers to pagemodel events
        void SubscribeToPageModelEvents()
        {
            OnLoaded += OnPageModelLoaded;
            OnResumed += OnPageModelResumed;
            OnClosed += OnPageModelClosed;
        }

        // Helper to raise any page event
        void RaisePageEvent(PageEventHandler handler, PageEventArgs e)
        {
            if(handler != null)
            {
                handler(this, e);
            }
        }

        // Unsubscribe handlers from pagemodel events
        void UnsubscribeFromPageModelEvents()
        {
            OnLoaded -= OnPageModelLoaded;
            OnResumed -= OnPageModelResumed;
            OnClosed -= OnPageModelClosed;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    RemoveBrowser();
                    UnsubscribeFromPageModelEvents();
                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }

    #region Page context

    /// <summary>
    /// Page context that wraps information to send to JS layer on page load
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct PageContext
    {
        [DefaultValue("")]
        [JsonProperty(PropertyName = "data", Required = Required.Default)]
        public string Data { get; set; }


        [JsonProperty(PropertyName = "orientation", Required = Required.Always)]
        public int Orientation { get; set; }

        [JsonProperty(PropertyName = "pageName", Required = Required.Always)]
        public string PageName { get; set; }

        [JsonProperty(PropertyName = "network", Required = Required.Always)]
        public bool Network { get; set; }

        
        [JsonProperty(PropertyName = "preferences", Required = Required.Always)]
        public Dictionary<string, object> Preferences { get; set; }

        [JsonProperty(PropertyName = "pageParent", Required = Required.AllowNull)]
        public string PageParent { get; set; }

        
        [JsonProperty(PropertyName = "waitingForResult", Required = Required.Always)]
        public bool WaitingForResult { get; set; }
    }

    #endregion

    #region Event definitions

    delegate void PageEventHandler(PageModel sender, PageEventArgs e);

    /// <summary>
    /// EventArgs that encapsulates event information from 
    /// <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> object
    /// </summary>
    class PageEventArgs : EventArgs
    {
        PageModel pageModel;

        Dictionary<string, object> properties;

        /// <summary>
        /// Properties of the event
        /// </summary>
        internal Dictionary<string, object> Properties { get { return properties ?? new Dictionary<string, object>(); } }

        /// <summary>
        /// Constructor - Default
        /// </summary>
        public PageEventArgs() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pageModel">
        /// <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> object
        ///  which raised this event
        /// </param>
        public PageEventArgs(PageModel pageModel) : this()
        {
            this.pageModel = pageModel;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <see cref="CloudPact.MowblyFramework.Core.Ui.PageModel">PageModel</see> object
        ///  which raised this event
        /// <param name="properties">Extended properties of the event</param>
        public PageEventArgs(PageModel pageModel, Dictionary<string, object> properties)
            : this()
        {
            this.properties = properties;
        }

        #endregion
 
    }
}