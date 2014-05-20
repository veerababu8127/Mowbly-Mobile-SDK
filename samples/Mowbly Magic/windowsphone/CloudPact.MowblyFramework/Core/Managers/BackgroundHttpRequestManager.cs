//-----------------------------------------------------------------------------------------
// <copyright file="BackgroundHttpRequestManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class BackgroundHttpRequestManager : IDisposable
    {
        #region Singleton

        bool isDisposed = false;

        static readonly Lazy<BackgroundHttpRequestManager> instance =
            new Lazy<BackgroundHttpRequestManager>(() => new BackgroundHttpRequestManager());

        BackgroundHttpRequestManager() { }

        ~BackgroundHttpRequestManager()
        {
            Dispose();
        }

        internal static BackgroundHttpRequestManager Instance { get { return instance.Value; } }

        #endregion

        #region Background HttpRequest Manager

        internal event EventHandler<BackgroundTransferEventArgs> TransferSuccess;

        internal event EventHandler<MowblyBackgroundTransferEventArgs> TransferError;

        internal event EventHandler<BackgroundTransferEventArgs> TransferProgressChanged;

        internal event EventHandler<BackgroundTransferEventArgs> TransferStatusChanged;

        #region Public methods

        internal bool Add(BackgroundTransferRequest request)
        {
            bool status = false;
            request.TransferProgressChanged += onTransferProgressChanged;
            request.TransferStatusChanged += onTransferStatusChanged;
            request.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            try
            {
                BackgroundTransferService.Add(request);
                status = true;
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error("Unable to add background transfer request. Reason - " + ex.Message);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to add background transfer request. Reason" + e.Message);
            }
            return status;
        }

        internal BackgroundTransferRequest Get(string requestId)
        {
            return BackgroundTransferService.Find(requestId);
        }

        internal void Reset()
        {
            // Remove all requests
            RemoveAllRequests();
        }

        internal void Remove(BackgroundTransferRequest request)
        {
            // Check if the request is in Background service before removing to
            // avoid Object aready disposed exception.
            if (BackgroundTransferService.Find(request.RequestId) != null)
            {
                request.TransferProgressChanged -= onTransferProgressChanged;
                request.TransferStatusChanged -= onTransferStatusChanged;
                BackgroundTransferService.Remove(request);
                request.Dispose();
            }
        }

        #endregion

        #region Private methods

        void onTransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            if (TransferProgressChanged != null)
            {
                TransferProgressChanged(sender, e);
            }
        }

        void onTransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            if (TransferStatusChanged != null)
            {
                TransferStatusChanged(sender, e);
            }

            BackgroundTransferRequest request = e.Request;
            switch(request.TransferStatus)
            {
                case TransferStatus.Completed:
                    if (request.StatusCode == 200 || request.StatusCode == 206)
                    {
                        if (TransferSuccess != null)
                        {
                            TransferSuccess(sender, e);
                        }
                    }
                    else
                    {
                        if (TransferError != null)
                        {
                            TransferError(sender, 
                                new MowblyBackgroundTransferEventArgs(
                                    e, e.Request.StatusCode.ToString()));
                        }
                    }

                    // Remove the request from queue
                    Remove(request);
                    break;
                case TransferStatus.Unknown:
                case TransferStatus.None:
                    // Unknown status, request is not usable. Remove from queue.
                    Logger.Error("Request [" + e.Request.RequestUri + "] has gone into status - " + request.TransferStatus);
                    Remove(request);
                    if (TransferError != null)
                    {
                        TransferError(sender, 
                            new MowblyBackgroundTransferEventArgs(
                                    e, Mowbly.GetString(Constants.STRING_UNKNOWN_ERROR)));
                    }
                    break;
                case TransferStatus.Waiting:
                case TransferStatus.WaitingForExternalPower:
                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                case TransferStatus.WaitingForWiFi:
                    Logger.Error("Request [" + e.Request.RequestUri + "] has gone into status - " + request.TransferStatus);
                    Remove(request);
                    if (TransferError != null)
                    {
                        TransferError(sender, 
                            new MowblyBackgroundTransferEventArgs(
                                    e, Mowbly.GetString(Constants.STRING_BACKGROUND_TRANSFER_WAITING)));
                    }
                    break;
                case TransferStatus.Paused:
                    Logger.Debug("Request [" + e.Request.RequestUri + "] paused");
                    break;
            }
        }

        void RemoveAllRequests()
        {
            IEnumerable<BackgroundTransferRequest> requests =
                        BackgroundTransferService.Requests;
            foreach (BackgroundTransferRequest request in requests)
            {
                Remove(request);
                request.Dispose();
            }
        }

        #endregion

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    RemoveAllRequests();
                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }

    class MowblyBackgroundTransferEventArgs
    {
        private BackgroundTransferRequest request;

        internal BackgroundTransferRequest Request
        {
            get
            {
                return request;
            }
        }

        private string errorText;

        internal string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
            }
        }

        internal MowblyBackgroundTransferEventArgs(BackgroundTransferEventArgs e)
        {
            this.request = e.Request;
        }

        internal MowblyBackgroundTransferEventArgs(BackgroundTransferEventArgs e, string errorText)
            : this(e)
        {
            this.errorText = errorText;
        }
    }
}