using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardListWindowVM
    {
        public ObservableProperty<string> Title { get; set; } = new ObservableProperty<string>("Cards");

        public CardsListVM CardList { get; set; } = new CardsListVM(false, true);

        public CardListWindowVM(string title, CardsListVM vmCardsList)
        {
            Title.Value = title;
            CardList = vmCardsList;
        }
}
}
