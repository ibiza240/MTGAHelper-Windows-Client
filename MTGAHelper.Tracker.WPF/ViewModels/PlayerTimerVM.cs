using System;
using System.Diagnostics;
using System.Windows.Threading;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class PlayerTimerVM : BasicModel
    {
        private readonly DispatcherTimer DispatcherTimer = new DispatcherTimer();

        private readonly Stopwatch StopWatch = new Stopwatch();

        public string TimePlayed
        {
            get
            {
                var ts = StopWatch.Elapsed;
                return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            }
        }

        public bool HasPriority => StopWatch.IsRunning;

        public PlayerTimerVM()
        {
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            DispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!StopWatch.IsRunning) return;

                OnPropertyChanged(nameof(TimePlayed));
                OnPropertyChanged(nameof(HasPriority));
            }
            catch (Exception)
            {
                Debugger.Break();
            }
        }

        public void Pause()
        {
            StopWatch.Stop();
            DispatcherTimer.Stop();
        }

        public void Resume()
        {
            StopWatch.Start();
            DispatcherTimer.Start();
        }

        public void Reset()
        {
            StopWatch.Reset();
            DispatcherTimer.Stop();
            OnPropertyChanged(nameof(TimePlayed));
        }
    }
}
