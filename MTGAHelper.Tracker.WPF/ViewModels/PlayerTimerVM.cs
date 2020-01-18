using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class PlayerTimerVM : ObservableObject
    {
        readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        readonly Stopwatch stopWatch = new Stopwatch();

        public string TimePlayed
        {
            get
            {
                TimeSpan ts = stopWatch.Elapsed;
                return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        public bool HasPriority => stopWatch.IsRunning;

        public PlayerTimerVM()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (stopWatch.IsRunning)
                {
                    RaisePropertyChangedEvent(nameof(TimePlayed));
                    RaisePropertyChangedEvent(nameof(HasPriority));
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        public void Pause()
        {
            stopWatch.Stop();
            dispatcherTimer.Stop();
        }

        public void Unpause()
        {
            stopWatch.Start();
            dispatcherTimer.Start();
        }

        public void Reset()
        {
            stopWatch.Reset();
            dispatcherTimer.Stop();
            RaisePropertyChangedEvent(nameof(TimePlayed));
        }
    }
}
