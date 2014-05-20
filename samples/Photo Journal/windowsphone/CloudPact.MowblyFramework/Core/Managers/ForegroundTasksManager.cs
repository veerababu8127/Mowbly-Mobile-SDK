//-----------------------------------------------------------------------------------------
// <copyright file="CustomForegroundTasksManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class CustomForegroundTasksManager : IForegroundTasksManager
    {
        bool isDisposed = false;

        CancellationTokenSource cancellationTokenSource =
            new CancellationTokenSource();

        #region Singleton

        static readonly Lazy<CustomForegroundTasksManager> instance =
            new Lazy<CustomForegroundTasksManager>(() => new CustomForegroundTasksManager());

        CustomForegroundTasksManager() { }

        internal static CustomForegroundTasksManager Instance { get { return instance.Value; } }

        #endregion

        #region IForegroundTasksManager

        List<Task> tasks = new List<Task>();

        public List<Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        public async void RunTasksAsync(IProgress<TaskProgress> progress)
        {
            // Set new cancellation token
            cancellationTokenSource = new CancellationTokenSource();

            // Trigger foreground tasks
            Logger.Debug("Running foreground tasks...");
            if (tasks.Count > 0)
            {
                Logger.Error("Cancelled foreground tasks. Reason - Pending tasks " + tasks.Count);
                return;
            }

            // Check updates
            Task checkUpdatesTask = Task.Run(() =>
            {
                CheckUpdatesAsync(progress);
            }, cancellationTokenSource.Token);
            tasks.Add(checkUpdatesTask);

            await checkUpdatesTask;
        }

        public void SuspendTasks()
        {
            // Cancel running tasks
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Private methods

        void CheckUpdatesAsync(IProgress<TaskProgress> progress)
        {
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    cancellationTokenSource.Dispose();
                    isDisposed = true;
                }
            }
            catch { }
        }

        #endregion
    }
}
