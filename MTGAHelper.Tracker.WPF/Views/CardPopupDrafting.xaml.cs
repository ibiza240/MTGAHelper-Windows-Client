using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;
using MTGAHelper.Tracker.WPF.Config;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopupDrafting
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CardPopupDrafting()
        {
            DataContext = ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// The view model and window data context
        /// </summary>
        private readonly DraftingCardPopupVM ViewModel = new DraftingCardPopupVM();

        #endregion

        #region Public Methods

        public void Refresh(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            ViewModel.SetDraftCard(cardVM, showGlobalMTGAHelperSays);
        }

        public void SetCardPopupPosition(CardPopupSide side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            var popupWidth = (int)Width;

            int toLeft = mainWindowLeft - popupWidth;
            int toRight = mainWindowLeft + mainWindowWidth;

            int leftAdjusted = side switch
            {
                CardPopupSide.Left => toLeft,
                CardPopupSide.Right => toRight,
                _ => (mainWindowLeft < SystemParameters.WorkArea.Width / 2 ? toRight : toLeft)
            };

            Top = mainWindowTop;
            Left = leftAdjusted;
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            ViewModel.SetPopupRatingsSource(showRatingsSource, source);
        }

        #endregion
    }
}
