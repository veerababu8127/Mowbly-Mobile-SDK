//-----------------------------------------------------------------------------------------
// <copyright file="AbstractLogHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Format;
using System.Diagnostics;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    class AbstractLogHandler : ILogHandler
    {
        public bool isDisposed = false;

        #region ILogHandler

        string name;

        bool isPropagationAllowed;

        LogLevel threshold;

        ILogFormatter formatter;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public bool IsPropagationAllowed
        {
            get
            {
                return isPropagationAllowed;
            }
            set
            {
                isPropagationAllowed = value;
            }
        }

        public LogLevel Threshold
        {
            get
            {
                return threshold;
            }
            set
            {
                threshold = value;
            }
        }

        public ILogFormatter Formatter
        {
            get
            {
                return formatter;
            }
            set
            {
                formatter = value;
            }
        }

        public virtual void Setup() { }

        public virtual void Start() { }

        public virtual void Handle(LogEvent e)
        {
            Debug.WriteLine(this.Formatter.Format(e));
        }

        public virtual void Shutdown() { }

        public virtual void Reset() { }

        #endregion

        #region Constructor

        public AbstractLogHandler()
        {
            this.name = "__log";
            this.isPropagationAllowed = true;
            this.threshold = LogLevel.DEBUG_LEVEL;
            this.formatter = new JsonFormatter();
        }

        ~AbstractLogHandler()
        {
            Dispose();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose() {}

        #endregion
    }
}