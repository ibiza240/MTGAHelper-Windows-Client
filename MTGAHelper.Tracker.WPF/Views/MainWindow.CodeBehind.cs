using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowCardPopupDrafting.SetCardPopupPosition(this.Top, this.Width);

            if (api.IsLocalTrackerUpToDate() == false)
                MustDownloadNewVersion();

            resourcesLocator.LocateLogFilePath(configApp);
            resourcesLocator.LocateGameClientFilePath(configApp);
            InitialServerApiCalls();
        }

        void MustDownloadNewVersion()
        {
#if DEBUG || DEBUGWITHSERVER
#else
            if (configApp.SkipVersionCheck)
                return;

            if (MessageBox.Show("A new version of the MTGAHelper Tracker is available, you must install it to continue. Proceed now?", "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var updaterApp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MTGAHelper.Tracker.AutoUpdater.exe");
                var ps = new ProcessStartInfo(updaterApp)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(ps);
            }

            App.Current.Shutdown();
#endif
        }

        void InitialServerApiCalls()
        {
            if (vm.CanUpload == false)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    vm.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
                    {
                        var collection = api.GetCollection(configApp.UserId);
                        vm.SetCollection(collection);
                    });

                    UploadLogFile();
                }
                catch (HttpRequestException ex)
                {
                    vm.SetProblemServerUnavailable();
                }
            });
        }

        void Window_Activated(object sender, EventArgs e)
        {
            Topmost = vm.AlwaysOnTop.Value;
            //if (vm.AlwaysOnTop)
            //{
            //    //Top = 0;
            //    //Left = 0;
            //}
        }

        void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = vm.AlwaysOnTop.Value;
            //if (vm.AlwaysOnTop)
            //{
            //    //Top = 0;
            //    //Left = 0;
            //    Activate();
            //}
        }

        void StatusBarTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.RefreshOpacity();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (double.IsNaN(Application.Current.MainWindow.Top) || double.IsNaN(Application.Current.MainWindow.Left))
                return;

            var left = (int)(this.Left < SystemParameters.WorkArea.Width / 2 ? this.Left + Width : this.Left - windowCardPopupDrafting.Width);
            windowCardPopupDrafting.SetCardPopupPosition((int)this.Top, left);
        }
    }
}
