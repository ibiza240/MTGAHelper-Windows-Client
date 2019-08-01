using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Ready.xaml
    /// </summary>
    public partial class Ready : UserControl
    {
        string gameFilePath;

        public Ready()
        {
            InitializeComponent();
        }

        public void Init(string gameFilePath)
        {
            this.gameFilePath = gameFilePath;
        }

        void HyperlinkLaunchGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appName = Path.GetFileNameWithoutExtension(gameFilePath);
                var processFound = Process.GetProcessesByName(appName).Length > 0;
                if (processFound)
                {
                    MessageBox.Show($"Game is already running.", "MTGAHelper");
                    return;
                }

                var ps = new ProcessStartInfo(gameFilePath);
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching the game: {ex.Message}", "MTGAHelper");
            }
        }
    }
}
