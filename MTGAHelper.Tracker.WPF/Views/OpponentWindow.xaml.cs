using System;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for OpponentWindow.xaml
    /// </summary>
    public partial class OpponentWindow
    {

        #region Public Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mvm"></param>
        public OpponentWindow(MainWindowVM mvm)
        {
            // Set the data context to the opponent window view model
            DataContext = mvm.OpponentWindowViewModel;

            InitializeComponent();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Set the location of the card pop-up
        /// </summary>
        /// <param name="side"></param>
        internal void SetCardsPopupPosition(ForceCardPopupSideEnum side)
        {
            CardsInWindow.SetCardPopupPosition(side, (int)Top, (int)Left, (int)Width);
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
                Point mousePos = PointToScreen(Mouse.GetPosition(this));
                Top = 0;
                Left = mousePos.X - 20;
                WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            DragMove();
        }

        /// <summary>
        /// Handle changes to the window location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!double.IsNaN(Top) && !double.IsNaN(Left))
                UpdatePosition();

        }

        /// <summary>
        /// Handle changes to the window size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!double.IsNaN(Width) && !double.IsNaN(Height))
                UpdatePosition();
        }

        /// <summary>
        /// Save the window position and size
        /// </summary>
        private void UpdatePosition()
        {
            CardsInWindow.SetCardPopupPosition(ForceCardPopupSideEnum.None, (int)Top, (int)Left, (int)Width);
        }

        #endregion
    }
}
