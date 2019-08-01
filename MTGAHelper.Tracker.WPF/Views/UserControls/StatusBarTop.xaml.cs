using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for StatusBarTop.xaml
    /// </summary>
    public partial class StatusBarTop : UserControl
    {
        MainWindow mainWindow => (MainWindow)Window.GetWindow(this);
        MainWindowVM vm => (MainWindowVM)mainWindow.DataContext;

        public StatusBarTop()
        {
            InitializeComponent();
        }

        private void Menu_Options_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowDialogOptions();
        }

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Menu_UploadNow_Click(object sender, RoutedEventArgs e)
        {
            if (vm.IsUploading)
            {
                MessageBox.Show($"The tracker is already uploading data, sorry for the slow speed. This waiting time can be greatly reduced with enough support, see how you can help on the website. Thanks!", "MTGAHelper");
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
            menuItemAlwaysOnTop.IsChecked = !menuItemAlwaysOnTop.IsChecked;
            vm.AlwaysOnTop.Value = menuItemAlwaysOnTop.IsChecked;
            mainWindow.Topmost = menuItemAlwaysOnTop.IsChecked;
            mainWindow.Activate();
        }
    }
}
