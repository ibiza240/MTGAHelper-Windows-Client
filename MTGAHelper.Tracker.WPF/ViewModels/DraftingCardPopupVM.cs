using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingCardPopupVM : ObservableObject
    {
        public CardDraftPickVM Card { get; set; } = new CardDraftPickVM();
        public ObservableProperty<bool> ShowGlobalMTGAHelperSays { get; set; } = new ObservableProperty<bool>(false);

        //public ObservableProperty<int> CardPopupTop { get; set; } = new ObservableProperty<int>(0);
        //public ObservableProperty<int> CardPopupLeft { get; set; } = new ObservableProperty<int>(0);

        public void SetDraftCard(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            Card = cardVM;
            ShowGlobalMTGAHelperSays.Value = showGlobalMTGAHelperSays;
            RaisePropertyChangedEvent(nameof(Card));
        }
    }
}
