using CloudPact.MowblyFramework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Managers
{
    /// <summary>
    /// Class that manages tasks that run when the app going to background
    /// </summary>
  
    interface IBackgroundTaskManager : IDisposable
    {
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
