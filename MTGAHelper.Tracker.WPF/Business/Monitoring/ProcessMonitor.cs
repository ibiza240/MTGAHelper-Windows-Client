using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business.Monitoring
{
    public class ProcessMonitor
    {
        public ProcessMonitor(
            MtgaProLoggerAdaptor mtgaProLoggerAdaptor
            )
        {
            this.mtgaProLoggerAdaptor = mtgaProLoggerAdaptor;
        }

        public Action<bool> OnProcessMonitorStatusChanged { get; set; }

        private const string PROCESS_NAME = "MTGA";
        private readonly MtgaProLoggerAdaptor mtgaProLoggerAdaptor;

        private bool IsRunning { get; set; }

        public async Task Start(CancellationToken cancellationToken)
        {
            await ContinuallyCheckForProcess(cancellationToken);
        }

        private Task ContinuallyCheckForProcess(CancellationToken cancellationToken)
        {
            Task task = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw new TaskCanceledException((Task)null);

                        bool processFound = System.Diagnostics.Process.GetProcessesByName(PROCESS_NAME).Length > 0;

                        bool stateChanged = processFound != IsRunning;

                        if (processFound == false && mtgaProLoggerAdaptor.IsStarted)
                        {
                            mtgaProLoggerAdaptor.Stop();
                        }

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
    }
}