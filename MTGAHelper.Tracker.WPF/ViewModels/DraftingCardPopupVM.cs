namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingCardPopupVM : ObservableObject
    {
        public CardDraftPickVM Card { get; set; } = new CardDraftPickVM();
        public ObservableProperty<bool> ShowGlobalMTGAHelperSays { get; set; } = new ObservableProperty<bool>(false);
        public string RatingsSource { get; set; }
        public bool ShowRatingsSource { get; set; }

        //public ObservableProperty<int> CardPopupTop { get; set; } = new ObservableProperty<int>(0);
        //public ObservableProperty<int> CardPopupLeft { get; set; } = new ObservableProperty<int>(0);

        public void SetDraftCard(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            Card = cardVM;
            ShowGlobalMTGAHelperSays.Value = showGlobalMTGAHelperSays;
            RaisePropertyChangedEvent(nameof(Card));
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            ShowRatingsSource = showRatingsSource;
            RatingsSource = source;

            RaisePropertyChangedEvent(nameof(ShowRatingsSource));
            RaisePropertyChangedEvent(nameof(RatingsSource));
        }

    }
}
