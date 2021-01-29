using System;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using Point = System.Windows.Point;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class OpponentWindow
    {
        public OpponentWindow(MainWindowVM mvm)
        {
            // Set the data context to the opponent window view model
            DataContext = mvm.OpponentWindowVM;

            InitializeComponent();
        }

        private void TabItemLabel_RefreshPossibleDeck(object sender, MouseButtonEventArgs e)
        {
            ((OpponentWindowVM)DataContext).RefreshDecksUsingCards();
        }

        internal void SetCardsPopupPosition(CardPopupSide side)
        {
            CardsInWindow.SetCardPopupPosition(side, (int)Top, (int)Left, (int)Width);
        }

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

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!double.IsNaN(Top) && !double.IsNaN(Left))
                UpdatePosition();

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!double.IsNaN(Width) && !double.IsNaN(Height))
                UpdatePosition();
        }

        private void UpdatePosition()
        {
            CardsInWindow.SetCardPopupPosition(CardPopupSide.Auto, (int)Top, (int)Left, (int)Width);
        }
    }
}
