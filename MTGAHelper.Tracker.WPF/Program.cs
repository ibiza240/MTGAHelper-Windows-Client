using System;
using System.Threading;
using System.Windows;

namespace MTGAHelper.Tracker.WPF
{
    static class Program
    {
        static Mutex mutex;
        [STAThread]
        static void Main()
        {
            // https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
            string mutexId = "Global\\{24eb5c47-62c6-4822-b305-b6265a3fbea3}";
            using (mutex = new Mutex(false, mutexId, out bool createdNew))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(1000, false);
                        if (hasHandle == false)
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
                    MessageBox.Show("Cannot run MTGAHelper more than once", "MTGAHelper");
                }
                finally
                {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }

}
