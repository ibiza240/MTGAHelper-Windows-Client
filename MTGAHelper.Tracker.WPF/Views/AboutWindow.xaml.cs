using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutWindow
    {
        #region Public Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Drag Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (WindowState == WindowState.Maximized)
            {
                var mousePos = PointToScreen(Mouse.GetPosition(this));
                Top = 0;
                Left = mousePos.X - 20;
                WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            DragMove();
        }

        /// <summary>
        /// Close the window on menu icon click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handle keypresses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Close the window on the escape key
            if (e.Key == Key.Escape) Close();
        }

        #endregion
    }
}
