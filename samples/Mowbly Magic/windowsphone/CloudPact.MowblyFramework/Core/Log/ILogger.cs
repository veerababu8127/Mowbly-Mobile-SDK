//-----------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Handler;
using System;
using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Log
{
    interface ILogger : IDisposable
    {
        #region Handler methods

        List<ILogHandler> Handlers { get; }

        void AddHandler(ILogHandler handler);

        ILogHandler GetHandler(string handler);

        void RemoveHandler(ILogHandler handler);

        void RemoveAllHandlers();

        void Reset();

        void Start();

        void Shutdown();

        #endregion

        #region Log methods

        string Name { get; }

        void Debug(string tag, string message);

        void Info(string tag, string message);

        void Warn(string tag, string message);

        void Error(string tag, string message);

        void Fatal(string tag, string message);

        #endregion
    }
}