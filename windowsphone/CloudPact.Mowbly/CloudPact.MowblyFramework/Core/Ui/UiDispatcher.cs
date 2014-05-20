//-----------------------------------------------------------------------------------------
// <copyright file="UiDispatcher.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;
using System.Windows;

namespace CloudPact.MowblyFramework.Core.Ui
{
    /// <summary>
    /// Helper class to run any code in UI thread.
    /// </summary>
    class UiDispatcher
    {
        internal static void BeginInvoke(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
        }
    }
}
