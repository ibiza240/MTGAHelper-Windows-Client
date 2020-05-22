using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;
using System.Threading.Tasks;
using MTGAHelper.Tracker.WPF.Business;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DraftHelper.xaml
    /// </summary>
    public partial class Drafting
    {
        private bool IsMouseOnCard = false;

        private MainWindow MainWindow => (MainWindow)Window.GetWindow(this);

        private MainWindowVM ViewModel => (MainWindowVM)MainWindow.DataContext;

        //DraftingCardPopupVM vmCardPopup = new DraftingCardPopupVM();

        private readonly CardPopupDrafting WindowCardPopupDrafting = new CardPopupDrafting();

        public readonly CardListPopup WindowCardsThatDidNotWheel = new CardListPopup();

        public Drafting()
        {
            InitializeComponent();

            // Disable tow selection
            //dataGrid.SelectionChanged += (obj, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        internal void Init(ICollection<Card> allCards, DraftingVM draftingVM, ServerApiCaller api, string limitedRatingsSource)
        {
            draftingVM.Init(allCards);
            WindowCardsThatDidNotWheel.Init(draftingVM.CardsThatDidNotWheelVM);
            DataContext = draftingVM;
            WindowCardPopupDrafting.Init(api, ViewModel.DraftingVM);
            ViewModel.DraftingVM.LimitedRatingsSource = limitedRatingsSource;
        }

        private void CardRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as CardDraftPickVM;
            WindowCardPopupDrafting.Refresh(vm, MainWindow.ViewModel.DraftingVM.ShowGlobalMTGAHelperSays);
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
            ViewModel.DraftingVM.ToggleShowHideCardListPopupThatDidNotWheel();

            WindowCardsThatDidNotWheel.Visibility = ViewModel.DraftingVM.ShowCardListThatDidNotWheel
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            WindowCardsThatDidNotWheel.Focus();
        }

        private void btnRunDraftHelper_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunDraftHelper();
        }

        public void PickCard(int grpId)
        {
            Dispatcher.Invoke(() =>
            {
                ViewModel.DraftingVM.DraftHumanPickCard(grpId);
                HideCardListWheeled();
            });
        }

        internal void HideCardListWheeled()
        {
            WindowCardsThatDidNotWheel.Visibility = Visibility.Hidden;
        }
    }
}
