using System.Diagnostics;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for StatusBarTop.xaml
    /// </summary>
    public partial class StatusBarTop
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusBarTop()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods
        
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

        #endregion
    }
}
