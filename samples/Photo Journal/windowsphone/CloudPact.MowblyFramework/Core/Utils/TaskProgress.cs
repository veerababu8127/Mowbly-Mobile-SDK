//-----------------------------------------------------------------------------------------
// <copyright file="TaskProgress.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CloudPact.MowblyFramework.Core.Utils
{
    class TaskProgress : Progress<TaskProgress>
    {
        public static int CODE_SUCCESS = 1;

        public static int CODE_ERROR = 0;

        #region Properties

        int code;

        bool status = true;

        string message;

        Dictionary<string, object> info;

        public int Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        public bool Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public string Message
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

        public Dictionary<string, object> Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
            }
        }

        #endregion

        #region Constructors

        public TaskProgress() { }

        public TaskProgress(int code, bool status)
        {
            this.code = code;
            this.status = status;
        }

        public TaskProgress(int code, string message)
            : this(code, true)
        {
            this.message = message;
        }

        public TaskProgress(int code, bool status, string message)
            : this(code, status)
        {
            this.message = message;
        }

        public TaskProgress(int code, bool status, string message, Dictionary<string, object> info)
            : this(code, status, message)
        {
            this.info = info;
        }

        #endregion
    }
}