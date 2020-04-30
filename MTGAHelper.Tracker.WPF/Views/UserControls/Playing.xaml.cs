using System;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Playing.xaml
    /// </summary>
    public partial class Playing
    {
        #region Private Fields

        /// <summary>
        /// External window for displaying opponent cards
        /// </summary>
        private OpponentWindow OpponentCardsWindow { get; set; }

        private MainWindowVM MainWindowVM { get; set; }

        /// <summary>
        /// Reference to the main window
        /// </summary>
        private MainWindow MainWindow => (MainWindow)Window.GetWindow(this);

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Playing()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function to initialize the class from DI
        /// </summary>
        /// <param name="mvm"></param>
        public void Init(MainWindowVM mvm)
        {
            // Create the opponent cards window
            OpponentCardsWindow = new OpponentWindow(mvm);

            // Set a reference to the main window view model
            MainWindowVM = mvm;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Set the card pop-up location
        /// </summary>
        /// <param name="side"></param>
        /// <param name="mainWindowTop"></param>
        /// <param name="mainWindowLeft"></param>
        /// <param name="mainWindowWidth"></param>
        internal void SetCardPopupPosition(ForceCardPopupSideEnum side, double mainWindowTop, double mainWindowLeft, double mainWindowWidth)
        {
            CardsMyLibrary.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            CardsMySideboard.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            CardsOpponent.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            OpponentCardsWindow.SetCardsPopupPosition(side);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Show the external opponents window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenOpponentCardsWindow(object sender, MouseButtonEventArgs e)
        {
            // If the external window option is not selected, do nothing and let the click change the tab
            if (!MainWindow.ConfigModel.ShowOpponentCardsExternal) return;

            // Set the already created window to visible
            MainWindowVM.OpponentWindowViewModel.IsWindowVisible = true;

            // Set the window as active
            OpponentCardsWindow.Activate();

            // Handle the click to prevent changing the tabs
            e.Handled = true;
        }

        #endregion
    }
}
