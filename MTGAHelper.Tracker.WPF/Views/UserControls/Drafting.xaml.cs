using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DraftHelper.xaml
    /// </summary>
    public partial class Drafting
    {
        private bool IsMouseOnCard = false;

        private MainWindow MainWindow => (MainWindow)Window.GetWindow(this);

        private MainWindowVM MainViewModel => (MainWindowVM)MainWindow.DataContext;
        private DraftingVM ViewModel => MainViewModel.DraftingVM;

        //DraftingCardPopupVM vmCardPopup = new DraftingCardPopupVM();

        private readonly CardPopupDrafting WindowCardPopupDrafting = new CardPopupDrafting();

        public readonly CardListPopup WindowCardsThatDidNotWheel = new CardListPopup();

        public Drafting()
        {
            InitializeComponent();

            // Disable tow selection
            //dataGrid.SelectionChanged += (obj, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        internal void Init(DraftingVM draftingVM, string limitedRatingsSource)
        {
            WindowCardsThatDidNotWheel.Init(draftingVM.CardsThatDidNotWheelVM);
            DataContext = draftingVM;
            WindowCardPopupDrafting.Init(ViewModel);
            ViewModel.LimitedRatingsSource = limitedRatingsSource;
        }

        private void CardRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as CardDraftPickVM;
            WindowCardPopupDrafting.Refresh(vm, MainWindow.ViewModel.DraftingVM.ShowGlobalMTGAHelperSays, MainWindow.ViewModel.Config.ShowAllDraftRatings, MainViewModel.DraftHelper.AllRatings);
            WindowCardPopupDrafting.ShowPopup(true);
            IsMouseOnCard = true;
        }

        private void CardRow_MouseLeave(object sender, MouseEventArgs e)
        {
            Task.Run(() =>
            {
                Task.Delay(400).Wait();
                if (IsMouseOnCard == false)
                    Dispatcher.Invoke(() => { WindowCardPopupDrafting.ShowPopup(false); });
            });
            IsMouseOnCard = false;
        }

        internal void SetCardPopupPosition(CardPopupSide side, int top, int left, int width)
        {
            WindowCardPopupDrafting.SetCardPopupPosition(side, top, left, width);
            WindowCardsThatDidNotWheel.SetCardPopupPosition(side, top, left, width);
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            WindowCardPopupDrafting.SetPopupRatingsSource(showRatingsSource, source);
        }

        private void ShowHideCardsThatDidNotWheel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.UpdateCardPopupPosition();
            ViewModel.ToggleShowHideCardListPopupThatDidNotWheel();

            WindowCardsThatDidNotWheel.Visibility = ViewModel.ShowCardListThatDidNotWheel
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            WindowCardsThatDidNotWheel.Focus();
        }

        //private void btnRunDraftHelper_Click(object sender, RoutedEventArgs e)
        //{
        //    MainWindow.RunDraftHelper();
        //}

        //public void PickCard(int grpId)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        ViewModel.DraftHumanPickCard(grpId);
        //        HideCardListWheeled();
        //    });
        //}

        internal void HideCardListWheeled()
        {
            WindowCardsThatDidNotWheel.Visibility = Visibility.Hidden;
        }
    }
}
