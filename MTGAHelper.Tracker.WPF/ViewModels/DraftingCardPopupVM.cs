using MTGAHelper.Tracker.WPF.Tools;

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

        #endregion

        #region Private Backing Fields

        private CardDraftPickVM _Card = new CardDraftPickVM();

        private bool _ShowGlobalMTGAHelperSays;

        private string _RatingsSource;

        private bool _ShowRatingsSource;

        private bool _ShowDescription;

        #endregion

        #region Public Methods

        public void SetDraftCard(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            Card = cardVM;
            ShowDescription = string.IsNullOrEmpty(Card.Description) == false;
            ShowGlobalMTGAHelperSays = showGlobalMTGAHelperSays;
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
