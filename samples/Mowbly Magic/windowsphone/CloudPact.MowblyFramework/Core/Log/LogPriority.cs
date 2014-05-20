//-----------------------------------------------------------------------------------------
// <copyright file="LogPriority.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;

namespace CloudPact.MowblyFramework.Core.Log
{
    class LogPriority : IComparable
    {
        internal enum Log_Priority
        {
            Debug = 10000,
            Info = 20000,
            Warn = 30000,
            Error = 40000,
            Fatal = 50000
        }

        int priority;

        string priorityStr;

        internal int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        internal string PriorityStr
        {
            get
            {
                return priorityStr;
            }
            set
            {
                priorityStr = value;
            }
        }

        public LogPriority(int priority, string priorityStr)
        {
            this.Priority = priority;
            this.PriorityStr = priorityStr;
        }

        internal bool GreaterThanOrEqualTo(object obj)
        {
            int comparisonResult = CompareTo(obj);
            return (comparisonResult >= 0);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            LogPriority otherLogPriority = obj as LogPriority;
            if (otherLogPriority != null)
            {
                return this.Priority.CompareTo(otherLogPriority.Priority);
            }
            else
            {
                throw new ArgumentException("Compare LogPriority failed. Object is not LogPriority.");
            }
        }

        public override string ToString()
        {
            return this.priorityStr;
        }
    }
}
