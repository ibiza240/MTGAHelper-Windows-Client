using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Playing.xaml
    /// </summary>
    public partial class Playing
    {
        private OpponentWindow OpponentCardsWindow { get; set; }

        private MainWindowVM MainWindowVM { get; set; }

        public Playing()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function to initialize the class from DI
        /// </summary>
        public void Init(MainWindowVM mvm)
        {
            // Create the opponent cards window
            OpponentCardsWindow = new OpponentWindow(mvm);

            // Set a reference to the main window view model
            MainWindowVM = mvm;
        }

        internal void SetCardPopupPosition(CardPopupSide side, double mainWindowTop, double mainWindowLeft, double mainWindowWidth)
        {
            CardsMyLibrary.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            CardsMySideboard.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            CardsOpponent.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            AllCardsOpponent.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            OpponentCardsWindow.SetCardsPopupPosition(side);
        }

        private void OpenOpponentCardsWindow(object sender, MouseButtonEventArgs e)
        {
            // If the external window option is not selected, do nothing and let the click change the tab
            if (!MainWindowVM.Config.ShowOpponentCardsExternal) return;

            // Show the opponent window
            MainWindowVM.OpponentWindowVM.ShowHideWindow(true);

            // Handle the click to prevent changing the tabs
            e.Handled = true;
        }

        private void CopyOpponentNameToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MainWindowVM.InMatchState.OpponentScreenName);
        }

        private void TabItemLabel_RefreshPossibleDeck(object sender, MouseButtonEventArgs e)
        {
            MainWindowVM.OpponentWindowVM.RefreshDecksUsingCards();
        }
    }
}
