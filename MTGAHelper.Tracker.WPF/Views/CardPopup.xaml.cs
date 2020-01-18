using System.Windows;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopup : Window
    {
        readonly CardPopupVM vm = new CardPopupVM();

        public CardPopup()
        {
            DataContext = vm;
            InitializeComponent();
        }

        internal void ShowCard(string imageCardUrl)
        {
            vm.ImageCardUrl.Value = imageCardUrl;
            this.Show();
        }
    }
}
