using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views
{
    public enum ForceCardPopupSideEnum
    {
        None,
        Left,
        Right,
    }

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

        public void SetCardPopupPosition(ForceCardPopupSideEnum side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            var popupWidth = (int)Width;

            int toLeft = mainWindowLeft - popupWidth;
            int toRight = mainWindowLeft + mainWindowWidth;

            int leftAdjusted = side switch
            {
                ForceCardPopupSideEnum.Left => toLeft,
                ForceCardPopupSideEnum.Right => toRight,
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
