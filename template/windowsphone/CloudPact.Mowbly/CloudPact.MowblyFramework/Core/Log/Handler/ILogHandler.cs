//-----------------------------------------------------------------------------------------
// <copyright file="ILogHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Format;
using System;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    interface ILogHandler : IDisposable
    {
        string Name { get; set; }

        bool IsPropagationAllowed { get; set; }

        LogLevel Threshold { get; set; }

        ILogFormatter Formatter { get; set; }

        void Setup();

        void Start();

        void Handle(LogEvent e);

        void Shutdown();

        void Reset();
    }
}