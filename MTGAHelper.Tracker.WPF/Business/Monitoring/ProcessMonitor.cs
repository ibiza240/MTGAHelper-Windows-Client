using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business.Monitoring
{
    public class ProcessMonitor
    {
        const string PROCESS_NAME = "MTGA";

        public bool IsRunning { get; private set; } = false;

        public event Action<object, bool> OnProcessMonitorStatusChanged;

        public ProcessMonitor()
        {
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await ContinuallyCheckForProcess(cancellationToken);
        }

        Task ContinuallyCheckForProcess(CancellationToken cancellationToken)
        {
            Task task = null;

            task = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested == true)
                            throw new TaskCanceledException(task);

                        var processFound = System.Diagnostics.Process.GetProcessesByName(PROCESS_NAME).Length > 0;
                        var stateChanged = processFound != IsRunning;
                        if (stateChanged)
                        {
                            IsRunning = processFound;
                            OnProcessMonitorStatusChanged?.Invoke(this, IsRunning);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log.Fatal(ex, "Unexpected error:");
                    }

                    Thread.Sleep(1000);
                }
            });

            return task;
        }
    }
}
