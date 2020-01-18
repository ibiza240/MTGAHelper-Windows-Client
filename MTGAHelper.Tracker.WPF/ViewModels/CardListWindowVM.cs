namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardListWindowVM
    {
        public ObservableProperty<string> Title { get; set; } = new ObservableProperty<string>("Cards");

        public CardsListVM CardList { get; set; } = new CardsListVM(false, true, CardsListOrder.Cmc);

        public CardListWindowVM(string title, CardsListVM vmCardsList)
        {
            Title.Value = title;
            CardList = vmCardsList;
        }
}
}
