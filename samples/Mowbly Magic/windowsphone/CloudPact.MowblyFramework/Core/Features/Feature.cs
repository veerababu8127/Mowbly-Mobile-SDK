//-----------------------------------------------------------------------------------------
// <copyright file="Feature.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    abstract class Feature : IDisposable
    {
        /// <summary>
        /// Flag to avoid duplicate disposing
        /// </summary>
        internal bool isDisposed;

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal virtual string Name { get; set; }

        

        /// <summary>
        /// Active page in the app
        /// </summary>
        internal PageModel Page
        {
            get { return PageManager.Instance.ActivePage; }
        }

        /// <summary>
        /// Invokes the method specified in the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage ">JSMessage</see> object.
        /// Will be overridden by the feature classes.
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage ">JSMessage</see> object
        /// </param>
        internal virtual async void InvokeAsync(JSMessage message) { await Task.FromResult(0); }

        /// <summary>
        /// Invokes the CallbackClient receive method in Mowbly javascript layer
        /// </summary>
        /// <param name="method">The method to call in Mowbly javascript layer</param>
        /// <param name="arg">Argument to be sent to the callback method mowbly javascript layer</param>
        internal void InvokeCallbackJavascript(string callbackId, object arg)
        {
            if (!string.IsNullOrEmpty(callbackId))
            {
                UiDispatcher.BeginInvoke(() =>
                {
                    Page.InvokeJavascript("__mowbly__.__CallbackClient", "onreceive", callbackId, arg);
                });
            }
        }

        /// <summary>
        /// Invokes the CallbackClient receive method in Mowbly javascript layer
        /// </summary>
        /// <param name="method">The method to call in Mowbly javascript layer</param>
        /// <param name="r">The 
        ///  <see cref="CloudPact.MowblyFramework.Core.Features.MethodResult">MethodResult</see>
        /// object to send to Mowbly javascript layer.</param>
        internal void InvokeCallbackJavascript(string callbackId, MethodResult r)
        {
            if (!string.IsNullOrEmpty(callbackId))
            {
                UiDispatcher.BeginInvoke(() =>
                {
                    Page.InvokeJavascript("__mowbly__.__CallbackClient", "onreceive", new Object[] { callbackId, r });
                });
            }
        }

        /// <summary>
        /// Invokes the specified method on the feature object in Mowbly javascript layer, with the result as argument.
        /// </summary>
        /// <param name="obj">The object to invoke the method on in Mowbly javascript layer</param>
        /// <param name="method">The method to call in Mowbly javascript layer</param>
        /// <param name="r">The 
        ///  <see cref="CloudPact.MowblyFramework.Core.Features.MethodResult">MethodResult</see>
        /// object to send to Mowbly javascript layer.</param>
        internal void InvokeJavascript(string obj, string method, MethodResult r)
        {
            UiDispatcher.BeginInvoke(() =>
            {
                Page.InvokeJavascript(obj, method, new Object[] { r });
            });
        }

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}