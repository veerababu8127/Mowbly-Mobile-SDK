using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Managers
{
    class BackgroundTasksManager : IBackgroundTaskManager
    {
        bool isDisposed = false;

        CancellationTokenSource cancellationTokenSource;

        #region Singleton

        static readonly Lazy<BackgroundTasksManager> instance =
            new Lazy<BackgroundTasksManager>(() => new BackgroundTasksManager());

        BackgroundTasksManager() { }

        internal static BackgroundTasksManager Instance { get { return instance.Value; } }

        #endregion

        #region IBackgroundTasksManager

        List<Task> tasks = new List<Task>();

        public List<Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        public void RunTasksAsync(IProgress<TaskProgress> progress)
        {
            // Set new cancellation token
            cancellationTokenSource = new CancellationTokenSource();

            // Trigger foreground tasks
            Logger.Debug("Running on background tasks...");
            if (tasks.Count > 0)
            {
                Logger.Error("Cancelled background tasks. Reason - Pending tasks " + tasks.Count);
                return;
            }

            // Start tasks
            tasks.ForEach(async (task) =>
            {
                try
                {
                    // Check if cancellation requested
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    // Run task
                    await task;

                    // Remove the task after it is run
                    tasks.Remove(task);
                    Logger.Debug("Completed background task. Pending tasks " + tasks.Count);
                }
                catch (Exception e)
                {
                    Logger.Error("Background tasks cancelled. Reason - " + e.Message);

                    // Remove task
                    tasks.Remove(task);
                }
            });
        }

        public void SuspendTasks()
        {
            // Cancel running tasks
            cancellationTokenSource.Cancel();
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
