using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for StatusBarTop.xaml
    /// </summary>
    public partial class StatusBarTop
    {
        #region Constructor / Initializer

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusBarTop()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="vm"></param>
        public void Init(MainWindow mainWindow, MainWindowVM vm)
        {
            MainWindow = mainWindow;
            MainWindowViewModel = vm;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Main Window
        /// </summary>
        private MainWindow MainWindow;

        /// <summary>
        /// Main Window View Model
        /// </summary>
        private MainWindowVM MainWindowViewModel;

        #endregion

        #region Private Methods

        /// <summary>
        /// Show the options dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Options_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ShowDialogOptions();
        }

        private void Menu_DraftHelperConfig_Click(object sender, RoutedEventArgs e)
        {
            string filepathConfigurator = Path.Combine(MainWindow.ConfigModel.DraftHelperFolder, "MTGAHelper.Tracker.DraftHelper.Configurator.exe");

            Log.Information($"Starting configurator at {filepathConfigurator}");

            // Use same Ratings source as in Options
            var configuratorConfigFilepath = Path.Combine(Path.GetDirectoryName(filepathConfigurator), "configurator.config.json");
            if (File.Exists(configuratorConfigFilepath))
            {
                var configuratorConfigFileContent = File.ReadAllText(configuratorConfigFilepath);
                var configuratorConfig = JsonConvert.DeserializeObject<ConfiguratorConfig>(configuratorConfigFileContent);
                if (configuratorConfig.RatingsSource != MainWindow.ConfigModel.ShowLimitedRatingsSource)
                {
                    configuratorConfig.RatingsSource = MainWindow.ConfigModel.ShowLimitedRatingsSource;
                    File.WriteAllText(configuratorConfigFilepath, JsonConvert.SerializeObject(configuratorConfig));
                }
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filepathConfigurator,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(filepathConfigurator)
                }
            };

            process.Start();
        }

        private void Menu_DraftHelperRun_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunDraftHelper();
        }

        /// <summary>
        /// Show the about dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow { Owner = Window.GetWindow(this) };
            about.ShowDialog();
        }

        /// <summary>
        /// Open the patch notes website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Close the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Upload the log file manually
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_UploadNow_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.Account.IsAuthenticated == false)
            {
                MessageBox.Show("Please sign-in first.", "MTGAHelper");
                return;
            }

            if (MainWindowViewModel.IsUploading)
            {
                MessageBox.Show($"The tracker is already uploading data, sorry for the slow speed. This waiting time can be greatly reduced, see how you can show your support on the website. Thanks!", "MTGAHelper");
                return;
            }

            MainWindow.UploadLogFile(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    string errors = MainWindowViewModel.ProblemsList.Count == 0 ? "." : $":{Environment.NewLine}{string.Join(Environment.NewLine, MainWindowViewModel.ProblemsList)}";
                    MessageBox.Show($"Could not upload the log file{errors}", "MTGAHelper");
                });
            });
        }

        private void Menu_Signin_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.SetMainWindowContext(WindowContext.Welcome);
        }

        /// <summary>
        /// Show the animated icon on an icon double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconClick(object sender, MouseButtonEventArgs e)
        {
            // Look for double clicks
            if (e.ClickCount != 2) return;

            // Flip the animated icon boolean
            MainWindowViewModel.AnimatedIcon = !MainWindowViewModel.AnimatedIcon;
        }

        #endregion
    }
}
