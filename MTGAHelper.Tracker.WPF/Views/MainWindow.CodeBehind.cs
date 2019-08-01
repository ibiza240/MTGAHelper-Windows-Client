using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Diagnostics;
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
            if (api.IsLocalTrackerUpToDate() == false)
                MustDownloadNewVersion();

            resourcesLocator.LocateLogFilePath(configApp);
            resourcesLocator.LocateGameClientFilePath(configApp);
            InitialServerApiCalls();
        }

        void MustDownloadNewVersion()
        {
            MessageBox.Show("A new version of the MTGAHelper tracker is available, please download it to access the latest features.", "MTGAHelper");
            var ps = new ProcessStartInfo(@"https://github.com/ibiza240/MTGAHelper-Windows-Client")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
            App.Current.Shutdown();
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
    }
}
