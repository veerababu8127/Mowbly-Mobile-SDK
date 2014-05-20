//-----------------------------------------------------------------------------------------
// <copyright file="AbstractLogger.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log.Handler;
using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Log
{
    class AbstractLogger : ILogger
    {
        bool isDisposed = false;

        List<ILogHandler> handlers = new List<ILogHandler>();

        string name;

        LogLevel threshold;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public List<ILogHandler> Handlers
        {
            get
            {
                return handlers;
            }
        }

        public AbstractLogger(string name)
        {
            this.name = name;
        }

        #region ILogger

        public void AddHandler(ILogHandler handler)
        {
            if(handler != null && !handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public ILogHandler GetHandler(string name)
        {
            ILogHandler matchedHandler = null;
            foreach(ILogHandler handler in handlers)
            {
                if(handler.Name.Equals(name)) {
                    matchedHandler = handler;
                    break;
                }
            }

            return matchedHandler;
        }

        public void RemoveHandler(ILogHandler handler)
        {
            if(handler != null && !handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }

        public void RemoveAllHandlers()
        {
            handlers.Clear();
        }

        public void Reset()
        {
            handlers.ForEach((handler) =>
                {
                    handler.Reset();
                });
        }

        public void Start()
        {
            handlers.ForEach((handler) =>
            {
                handler.Start();
            });
        }

        public void Shutdown()
        {
            handlers.ForEach((handler) =>
            {
                handler.Shutdown();
            });
        }

        public void Debug(string tag, string message)
        {
            Log(tag, message, LogLevel.DEBUG_LEVEL);
        }

        public void Info(string tag, string message)
        {
            Log(tag, message, LogLevel.INFO_LEVEL);
        }

        public void Warn(string tag, string message)
        {
            Log(tag, message, LogLevel.WARN_LEVEL);
        }

        public void Error(string tag, string message)
        {
            Log(tag, message, LogLevel.ERROR_LEVEL);
        }

        public void Fatal(string tag, string message)
        {
            Log(tag, message, LogLevel.FATAL_LEVEL);
        }

        void Log(string tag, string message, LogLevel level)
        {
            
            LogEvent e = new LogEvent(level, tag, message);
            foreach (ILogHandler handler in handlers)
            {
                if (level.GreaterThanOrEqualTo(handler.Threshold))
                {
                    handler.Handle(e);
                    if (!handler.IsPropagationAllowed || e.IsHandled)
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    foreach (ILogHandler handler in handlers)
                    {
                        handler.Dispose();
                    }
                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }
}
