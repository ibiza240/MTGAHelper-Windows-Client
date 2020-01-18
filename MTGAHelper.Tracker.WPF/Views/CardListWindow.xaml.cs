using System;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardListPopup.xaml
    /// </summary>
    public partial class CardListWindow : Window
    {
        readonly CardListWindowVM vm;
        readonly MainWindow mainWindow;

        public CardListWindow(string title, CardsListVM vmCardList, MainWindow mainWindow)
        {
            vm = new CardListWindowVM(title, vmCardList);
            this.DataContext = vm;
            this.mainWindow = mainWindow;

            InitializeComponent();
        }

        private void lblTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.WindowState == WindowState.Maximized)
            {
                var mousePos = PointToScreen(Mouse.GetPosition(this));
                this.Top = 0;
                this.Left = mousePos.X - 20;
                this.WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            this.DragMove();
        }

        internal void SetCardsPopupPosition(ForceCardPopupSideEnum side)
        {
            CardsInWindow.SetCardPopupPosition(side, (int)Top, (int)Left, (int)Width);
        }

        private void LabelClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!double.IsNaN(this.Top) && !double.IsNaN(this.Left))
                UpdatePosition();

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeToContent = SizeToContent.Height;

            if (!double.IsNaN(this.Width))
                UpdatePosition();
        }

        void UpdatePosition()
        {
            //CardsInWindow.SetCardPopupPosition(mainWindow.GetCardPopupSide(), (int)Top, (int)Left, (int)Width);
            CardsInWindow.SetCardPopupPosition(ForceCardPopupSideEnum.None, (int)Top, (int)Left, (int)Width);
            mainWindow.UpdateWindowOpponentCardsPosition((int)this.Top, (int)this.Left, (int)this.Width);
        }
    }
}
