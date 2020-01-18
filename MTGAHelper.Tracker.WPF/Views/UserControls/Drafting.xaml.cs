using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DraftHelper.xaml
    /// </summary>
    public partial class Drafting : UserControl
    {
        MainWindow mainWindow => (MainWindow)Window.GetWindow(this);
        MainWindowVM vm => (MainWindowVM)mainWindow.DataContext;
        //DraftingCardPopupVM vmCardPopup = new DraftingCardPopupVM();

        readonly CardPopupDrafting windowCardPopupDrafting = new CardPopupDrafting();
        readonly CardListPopup windowCardListWheeled = new CardListPopup();

        public Drafting()
        {
            InitializeComponent();

            // Disable tow selection
            //dataGrid.SelectionChanged += (obj, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        internal void Init(ICollection<Card> allCards, DraftingVM draftingVM)
        {
            draftingVM.Init(allCards);
            windowCardListWheeled.Init(draftingVM.CardsThatDidntWheelVM);
            this.DataContext = draftingVM;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void CardRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as CardDraftPickVM;
            windowCardPopupDrafting.Refresh(vm, mainWindow.vm.DraftingVM.ShowGlobalMTGAHelperSays);
            windowCardPopupDrafting.Visibility = Visibility.Visible;

        }

        private void CardRow_MouseLeave(object sender, MouseEventArgs e)
        {
            windowCardPopupDrafting.Visibility = Visibility.Hidden;
        }

        internal void SetCardPopupPosition(ForceCardPopupSideEnum side, int top, int left, int width)
        {
            windowCardPopupDrafting.SetCardPopupPosition(side, top, left, width);
            windowCardListWheeled.SetCardPopupPosition(side, top, left, width);
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            windowCardPopupDrafting.SetPopupRatingsSource(showRatingsSource, source);
        }

        private void ShowHideCardsThatDidntWheel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (vm.DraftingVM.ShowCardListThatDidntWheel.Value == false)
            //{
            //    // Setup the cardlist popup content
            //    var cardsWheeled = allCards
            //        .Where(i => vm.DraftingVM.CardsThatDidntWheel.Contains(i.grpId))
            //        .Select(i => Mapper.Map<CardWpf>(i))
            //        .ToArray();

            //    windowCardListWheeled.SetDataContext(vm.DraftingVM.CardChosenThatDidntWheel, cardsWheeled);
            //}

            vm.DraftingVM.ToggleShowHideCardListPopupThatDidntWheel();
            windowCardListWheeled.Visibility = vm.DraftingVM.ShowCardListThatDidntWheel.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            windowCardListWheeled.Focus();
        }
    }
}
