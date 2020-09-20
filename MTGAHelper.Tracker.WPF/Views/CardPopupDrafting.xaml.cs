using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;
using MTGAHelper.Tracker.WPF.Config;
using System.Collections;
using System.Collections.Generic;
using System;
using MTGAHelper.Tracker.WPF.Business;
using System.Windows.Input;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopupDrafting
    {
        #region Private Fields

        /// <summary>
        /// The view model and window data context
        /// </summary>
        private readonly DraftingCardPopupVM ViewModel = new DraftingCardPopupVM();

        private DraftingVM ViewModelDrafting;

        #endregion

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

        #region Public Methods

        public void Init(DraftingVM draftingVM)
        {
            this.ViewModelDrafting = draftingVM;
        }

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

        public void ShowPopup(bool isVisible)
        {
            if (ViewModel.IsMouseInPopup == false)
                ViewModel.IsPopupVisible = isVisible;
        }
        #endregion

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.IsMouseInPopup = true;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.IsMouseInPopup = false;
            ViewModel.IsPopupVisible = false;

            SaveCustomRating();
        }

        private void SaveCustomRating()
        {
            var customRatingInitialized = ViewModel.Card.CustomRatingValue != null || ViewModel.CustomRatingSelected != 0;

            var valuesChanged = (customRatingInitialized && ViewModel.Card.CustomRatingValue != ViewModel.CustomRatingSelected) ||
                ViewModel.Card.CustomRatingDescription != ViewModel.CustomRatingDescription;

            if (valuesChanged)
            {
                Task.Factory.StartNew(() =>
                {
                    ViewModel.Card.CustomRatingDescription = ViewModel.CustomRatingDescription;
                    ViewModel.Card.CustomRatingValue = ViewModel.CustomRatingSelected;

                    ViewModelDrafting.RefreshCardsDraft();
                    ViewModelDrafting.Api.SaveCustomDraftRating(ViewModel.Card.ArenaId, ViewModel.CustomRatingSelected, ViewModel.CustomRatingDescription);
                });
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // On startup apparently
            if (ViewModelDrafting == null)
                return;

            if (ViewModelDrafting.LimitedRatingsSource == Constants.LIMITEDRATINGS_SOURCE_CUSTOM)
            {
                ViewModel.Card.RatingValue = ViewModel.CustomRatingSelected;
                ViewModel.Card.RatingToDisplay = ViewModel.CustomRatingSelected.ToString();
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Just remove the cursor from textbox for some feedback
                Keyboard.ClearFocus();
            }
        }
    }
}
