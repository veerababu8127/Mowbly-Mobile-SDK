//-----------------------------------------------------------------------------------------
// <copyright file="LogEvent.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Utils;
using System;

namespace CloudPact.MowblyFramework.Core.Log
{
    class LogEvent
    {
        #region Properties

        bool isHandled = false;

        LogLevel level;

        string tag;

        string message;

        string type;

        string username;

        string space;

        internal bool IsHandled
        {
            get
            {
                return isHandled;
            }
            set
            {
                isHandled = value;
            }
        }

        internal LogLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        internal string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        internal string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        internal string Type
        {
            get
            {
                return (String.IsNullOrEmpty(type)) ?
                    LoggerManager.TYPE_SYSTEM_LOGGER : type;
            }
            set
            {
                type = value;
            }
        }

        internal string Username
        {
            get
            {
                return (String.IsNullOrEmpty(username)) ?
                    "System" : username;
            }
            set
            {
                username = value;
            }
        }

        internal string Space
        {
            get
            {
                return (String.IsNullOrEmpty(space)) ?
                    "" : space;
            }
            set
            {
                space = value;
            }
        }

        internal long TimeStamp
        {
            get
            {
                return DateTimeUtils.GetEpochTimestamp();
            }
        }

        #endregion

        public LogEvent(LogLevel level, string tag, string message)
        {
            this.Level = level;
            this.Tag = tag;
            this.Message = message;
        }
    }
}