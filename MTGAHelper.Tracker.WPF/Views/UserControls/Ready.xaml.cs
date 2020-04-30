using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Ready.xaml
    /// </summary>
    public partial class Ready : UserControl
    {
        private string GameFilePath;

        public Ready()
        {
            InitializeComponent();
        }

        public void Init(string gameFilePath)
        {
            GameFilePath = gameFilePath;
        }

        private void HyperlinkLaunchGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string? appName = Path.GetFileNameWithoutExtension(GameFilePath);
                bool processFound = Process.GetProcessesByName(appName).Length > 0;
                if (processFound)
                {
                    MessageBox.Show($"Game is already running.", "MTGAHelper");
                    return;
                }

                var ps = new ProcessStartInfo(GameFilePath);
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching the game: {ex.Message}", "MTGAHelper");
            }
        }
    }
}
