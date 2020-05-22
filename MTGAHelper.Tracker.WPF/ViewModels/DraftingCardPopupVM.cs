using MTGAHelper.Tracker.WPF.Tools;
using System.Collections.Generic;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingCardPopupVM : BasicModel
    {
        #region Public Properties

        public CardDraftPickVM Card
        {
            get => _Card;
            set => SetField(ref _Card, value, nameof(Card));
        }

        public bool ShowGlobalMTGAHelperSays
        {
            get => _ShowGlobalMTGAHelperSays;
            set => SetField(ref _ShowGlobalMTGAHelperSays, value, nameof(ShowGlobalMTGAHelperSays));
        }

        public string RatingsSource
        {
            get => _RatingsSource;
            set => SetField(ref _RatingsSource, value, nameof(RatingsSource));
        }

        public bool ShowRatingsSource
        {
            get => _ShowRatingsSource;
            set => SetField(ref _ShowRatingsSource, value, nameof(ShowRatingsSource));
        }

        public bool ShowDescription
        {
            get => _ShowDescription;
            set => SetField(ref _ShowDescription, value, nameof(ShowDescription));
        }

        public bool ShowCustomRating
        {
            get => _ShowCustomRating;
            set => SetField(ref _ShowCustomRating, value, nameof(ShowCustomRating));
        }

        public bool IsPopupVisible
        {
            get => _IsPopupVisible;
            set => SetField(ref _IsPopupVisible, value, nameof(IsPopupVisible));
        }

        public bool IsMouseInPopup
        {
            get => _IsMouseInPopup;
            set => SetField(ref _IsMouseInPopup, value, nameof(IsMouseInPopup));
        }

        public string CustomRatingDescription
        {
            get => _CustomRatingDescription;
            set => SetField(ref _CustomRatingDescription, value, nameof(CustomRatingDescription));
        }

        public int CustomRatingSelected
        {
            get => _CustomRatingSelected;
            set => SetField(ref _CustomRatingSelected, value, nameof(CustomRatingSelected));
        }

        public ICollection<int> CustomRatings { get; } = new[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };

        #endregion

        #region Private Backing Fields

        private CardDraftPickVM _Card = new CardDraftPickVM();

        private bool _ShowGlobalMTGAHelperSays;

        private string _RatingsSource;

        private bool _ShowRatingsSource;

        private bool _ShowDescription;

        private bool _ShowCustomRating;

        private bool _IsPopupVisible;

        private bool _IsMouseInPopup;

        private string _CustomRatingDescription;

        private int _CustomRatingSelected;

        #endregion

        #region Public Methods

        public void SetDraftCard(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            Card = cardVM;
            CustomRatingSelected = Card.CustomRatingValue ?? 0;
            CustomRatingDescription = Card.CustomRatingDescription;
            ShowDescription = string.IsNullOrEmpty(Card.Description) == false;
            ShowGlobalMTGAHelperSays = showGlobalMTGAHelperSays;
            ShowCustomRating = cardVM.CustomRatingValue != null || cardVM.CustomRatingDescription != null;
            OnPropertyChanged(nameof(Card));
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            ShowRatingsSource = showRatingsSource;
            RatingsSource = source;
        }

        #endregion
    }
}
