using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for StatusBarTop.xaml
    /// </summary>
    public partial class StatusBarTop : UserControl
    {
        MainWindow mainWindow;// => (MainWindow)Window.GetWindow(this);
        MainWindowVM vm;

        //DraftHelper draftHelper;
        //LogProcessor logProcessor;
        //string userId;

        public StatusBarTop()
        {
            InitializeComponent();
        }

        public StatusBarTop Init(MainWindow mainWindow, MainWindowVM vm/*, DraftHelper draftHelper,LogProcessor logProcessor, string userId,*/)
        {
            this.mainWindow = mainWindow;
            this.vm = vm;
            //this.draftHelper = draftHelper;
            //this.logProcessor = logProcessor;
            //this.userId = userId;
            //this.allCards = allCards;
            return this;
        }

        private void Menu_Options_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowDialogOptions();
        }

        private void Menu_Minimize_Click(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
        }

        public void MinimizeWindow()
        {
            if (mainWindow.configApp.MinimizeToSystemTray)
            {
                //mainWindow.ShowInTaskbar = false;
                //mainWindow.trayIcon.Visible = true;
                mainWindow.Visibility = Visibility.Hidden;
            }
            else
                mainWindow.WindowState = WindowState.Minimized;
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.Owner = Window.GetWindow(this);
            about.ShowDialog();
        }

        private void Menu_PatchNotes_Click(object sender, RoutedEventArgs e)
        {
            var ps = new ProcessStartInfo("https://github.com/ibiza240/MTGAHelper-Windows-Client/blob/master/PatchNotes.md")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
            e.Handled = true;
        }

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Menu_UploadNow_Click(object sender, RoutedEventArgs e)
        {
            if (vm.Account.IsAuthenticated == false)
            {
                MessageBox.Show("Please sign-in first.", "MTGAHelper");
                return;
            }

            if (vm.IsUploading)
            {
                MessageBox.Show($"The tracker is already uploading data, sorry for the slow speed. This waiting time can be greatly reduced, see how you can show your support on the website. Thanks!", "MTGAHelper");
                return;
            }

            mainWindow.UploadLogFile(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    var errors = vm.ProblemsList.Count == 0 ? "." : $":{Environment.NewLine}{string.Join(Environment.NewLine, vm.ProblemsList)}";
                    MessageBox.Show($"Could not upload the log file{errors}", "MTGAHelper");
                });
            });
        }

        private void Menu_AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.SetAlwaysOnTop(!menuItemAlwaysOnTop.IsChecked);
            mainWindow.Activate();
        }

        internal void SetAlwaysOnTop(bool alwaysOnTop)
        {
            menuItemAlwaysOnTop.IsChecked = alwaysOnTop;
            vm.AlwaysOnTop.Value = alwaysOnTop;
            mainWindow.Topmost = alwaysOnTop;
        }

        private void Menu_Signin_Click(object sender, RoutedEventArgs e)
        {
            vm.SetMainWindowContext(MainWindowContextEnum.Welcome);
        }

        //int i = 0;
        //private void MenuTestDrafter_Click(object sender, RoutedEventArgs e)
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        if (i % 2 == 0)
        //        {
        //            var draftPack = "\"69195\",\"69245\",\"69212\",\"69261\",\"69382\",\"69268\",\"69172\",\"69278\",\"69299\",\"69312\",\"69248\",\"69339\",\"69287\",\"69155\",\"69400\"";

        //            var rng = new Random();
        //            var randomizedCards = allCards
        //                .OrderBy(i => rng.NextDouble())
        //                .Take(15)
        //                .Select(i => "\"" + i.grpId + "\"")
        //                .ToArray();
        //            draftPack = string.Join(",", randomizedCards);

        //            var msg = "[UnityCrossThreadLogger]8/9/2019 1:05:03 AM" + Environment.NewLine + "<== Draft.DraftStatus(113)" + Environment.NewLine +
        //"{\"playerId\":\"933E5CE627155485\",\"eventName\":\"QuickDraft_RNA_20190802\",\"draftId\":\"933E5CE627155485: QuickDraft_RNA_20190802: Draft\",\"draftStatus\":\"Draft.PickNext\",\"packNumber\":0,\"pickNumber\":0,\"draftPack\":[" + draftPack + "],\"pickedCards\":[],\"requestUnits\":0.0}";

        //            mainWindow.OnFileSizeChangedNewText(this, msg);
        //        }
        //        else
        //        {
        //            var msg = File.ReadAllText(@"D:\repos\MTGAHelper\CompleteDraft.txt");
        //            mainWindow.OnFileSizeChangedNewText(this, msg);
        //        }

        //        i++;
        //    });
        //}
    }
}
