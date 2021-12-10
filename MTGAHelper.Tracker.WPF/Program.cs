using System;
using System.Threading;
using System.Windows;
using Serilog;

namespace MTGAHelper.Tracker.WPF
{
    internal static class Program
    {
        private static Mutex Mutex;

        [STAThread]
        private static void Main()
        {
            // https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
            const string mutexId = "Global\\{24eb5c47-62c6-4822-b305-b6265a3fbea3}";
            using (Mutex = new Mutex(false, mutexId, out _))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = Mutex.WaitOne(1000, false);
                        if (hasHandle == false)
                            if (MessageBox.Show("MTGAHelper is already running! Are you sure you want to continue?", "Other MTGAHelper instance detected!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                                throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        hasHandle = true;
                    }

                    var application = new App();
                    application.InitializeComponent();
                    application.Run();
                }
                catch (TimeoutException)
                {
                    //MessageBox.Show("Cannot run MTGAHelper more than once", "MTGAHelper");
                }
                catch (Exception e)
                {
                    Log.Fatal(e, "Fatal error");
                }
                finally
                {
                    if (hasHandle)
                        Mutex.ReleaseMutex();
                }
            }
        }
    }
}