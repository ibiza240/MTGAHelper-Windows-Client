using System.Diagnostics;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    public partial class StatusBarTop
    {
        public StatusBarTop()
        {
            InitializeComponent();
        }

        private void Menu_GoToWebsite_Click(object sender, RoutedEventArgs e)
        {
            var ps = new ProcessStartInfo("https://mtgahelper.com")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
            e.Handled = true;
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow { Owner = Window.GetWindow(this) };
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
    }
}
