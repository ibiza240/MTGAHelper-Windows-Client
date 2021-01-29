using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class CardPopup
    {
        public CardPopup()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
        private readonly CardPopupVM ViewModel = new CardPopupVM();

        internal void ShowCard(string imageCardUrl)
        {
            ViewModel.ImageCardUrl = imageCardUrl;

            Show();
        }
    }
}
