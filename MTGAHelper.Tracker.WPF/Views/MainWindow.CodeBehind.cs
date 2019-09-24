using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowCardPopupDrafting.SetCardPopupPosition((int)this.Top, (int)this.Left, (int)windowCardPopupDrafting.Width);

            if (api.IsLocalTrackerUpToDate() == false)
                MustDownloadNewVersion();

            InitialServerApiCalls();
            //InitStoryBoard();
        }

        void MustDownloadNewVersion()
        {
#if DEBUG || DEBUGWITHSERVER
#else
            if (MessageBox.Show("A new version of the MTGAHelper Tracker is available, you must install it to continue. Proceed now?", "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Download latest auto-updater
                var folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
                var fileExe = Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.exe");
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.dll", Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.dll"));
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.exe", fileExe);
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.runtimeconfig.json", Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.runtimeconfig.json"));

                var ps = new ProcessStartInfo(fileExe)
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

        void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        void StatusBarTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        //Storyboard storyboardHighlightCard = new Storyboard();
        //private void InitStoryBoard()
        //{
        //    // Create a NameScope for the page so
        //    // that Storyboards can be used.
        //    NameScope.SetNameScope(this, new NameScope());

        //    // Background="#272b30"
        //    var animatedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#272b30"));
        //    this.Background = animatedBrush;

        //    // Register the brush's name with the page
        //    // so that it can be targeted by storyboards.
        //    this.RegisterName("MyAnimatedBrush", animatedBrush);

        //    ColorAnimation highlightCardColorAnimation = new ColorAnimation();
        //    highlightCardColorAnimation.From = Colors.Blue;
        //    //highlightCardColorAnimation.To = Colors.Red;
        //    highlightCardColorAnimation.Duration = TimeSpan.FromSeconds(1);
        //    Storyboard.SetTargetName(highlightCardColorAnimation, "MyAnimatedBrush");
        //    Storyboard.SetTargetProperty(highlightCardColorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
        //    storyboardHighlightCard.Children.Add(highlightCardColorAnimation);

        //    storyboardHighlightCard.Begin(this);
        //}

        void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.RefreshOpacity();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (double.IsNaN(Application.Current.MainWindow.Top) || double.IsNaN(Application.Current.MainWindow.Left))
                return;

            windowCardPopupDrafting.SetCardPopupPosition((int)this.Top, (int)this.Left, (int)windowCardPopupDrafting.Width);
        }
    }
}
