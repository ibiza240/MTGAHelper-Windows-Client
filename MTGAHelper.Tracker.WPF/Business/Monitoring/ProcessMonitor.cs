using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business.Monitoring
{
    public class ProcessMonitor
    {
        #region Public Properties

        /// <summary>
        /// Process Status Changed Action
        /// </summary>
        public Action<bool> OnProcessMonitorStatusChanged { get; set; }

        #endregion

        #region Private Fields

        /// <summary>
        /// Constant process name
        /// </summary>
        private const string PROCESS_NAME = "MTGA";

        /// <summary>
        /// Whether the process is running
        /// </summary>
        private bool IsRunning { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the async monitoring task
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Start(CancellationToken cancellationToken)
        {
            await ContinuallyCheckForProcess(cancellationToken);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Task for continuously monitoring a process of status changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task ContinuallyCheckForProcess(CancellationToken cancellationToken)
        {
            Task task = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw new TaskCanceledException((Task) null);

                        bool processFound = System.Diagnostics.Process.GetProcessesByName(PROCESS_NAME).Length > 0;

                        bool stateChanged = processFound != IsRunning;

                        if (stateChanged)
                        {
                            IsRunning = processFound;
                            OnProcessMonitorStatusChanged?.Invoke(IsRunning);
                        }
                    }
                    catch (Exception)
                    {
                        //Log.Fatal(ex, "Unexpected error:");
                    }

                    Thread.Sleep(1000);
                }
            }, cancellationToken);

            return task;
        }

        #endregion

    }
}
