using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DraftHelper.xaml
    /// </summary>
    public partial class Drafting
    {
        private MainWindow MainWindow => (MainWindow)Window.GetWindow(this);

        private MainWindowVM ViewModel => (MainWindowVM)MainWindow.DataContext;

        //DraftingCardPopupVM vmCardPopup = new DraftingCardPopupVM();

        private readonly CardPopupDrafting WindowCardPopupDrafting = new CardPopupDrafting();

        private readonly CardListPopup WindowCardListWheeled = new CardListPopup();

        public Drafting()
        {
            InitializeComponent();

            // Disable tow selection
            //dataGrid.SelectionChanged += (obj, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        internal void Init(ICollection<Card> allCards, DraftingVM draftingVM)
        {
            draftingVM.Init(allCards);
            WindowCardListWheeled.Init(draftingVM.CardsThatDidNotWheelVM);
            DataContext = draftingVM;
        }

        private void CardRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as CardDraftPickVM;
            WindowCardPopupDrafting.Refresh(vm, MainWindow.MainWindowVM.DraftingVM.ShowGlobalMTGAHelperSays);
            WindowCardPopupDrafting.Visibility = Visibility.Visible;

        }

        private void CardRow_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowCardPopupDrafting.Visibility = Visibility.Hidden;
        }

        internal void SetCardPopupPosition(ForceCardPopupSideEnum side, int top, int left, int width)
        {
            WindowCardPopupDrafting.SetCardPopupPosition(side, top, left, width);
            WindowCardListWheeled.SetCardPopupPosition(side, top, left, width);
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            WindowCardPopupDrafting.SetPopupRatingsSource(showRatingsSource, source);
        }

        private void ShowHideCardsThatDidNotWheel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.DraftingVM.ToggleShowHideCardListPopupThatDidNotWheel();

            WindowCardListWheeled.Visibility = ViewModel.DraftingVM.ShowCardListThatDidNotWheel
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            WindowCardListWheeled.Focus();
        }

        private void btnRunDraftHelper_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunDraftHelper();
        }
    }
}
