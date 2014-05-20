//-----------------------------------------------------------------------------------------
// <copyright file="IForegroundTasksManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Managers
{
    /// <summary>
    /// Class that manages tasks that run when the app comes to foreground
    /// </summary>
    interface IForegroundTasksManager : IDisposable
    {
        /// <summary>
        /// List of tasks
        /// </summary>
        List<Task> Tasks { get; set; }

        /// <summary>
        /// Queue and run the tasks
        /// </summary>
        void RunTasksAsync(IProgress<TaskProgress> progress);

        /// <summary>
        /// Stop running tasks
        /// </summary>
        void SuspendTasks();
    }
}
