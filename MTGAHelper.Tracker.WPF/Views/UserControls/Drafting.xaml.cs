using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        public Drafting()
        {
            InitializeComponent();

            // Disable tow selection
            //dataGrid.SelectionChanged += (obj, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        internal void Init(DraftingVM draftingVM)
        {
            this.DataContext = draftingVM;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void CardThumbnail_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as CardDraftPickVM;
            //mainWindow.windowCardPopup.imgPopupCard.Source = new ImageSource( vm.ImageCardUrl;
            mainWindow.windowCardPopupDrafting.Refresh(vm, mainWindow.vm.DraftingVM.ShowGlobalMTGAHelperSays);
            mainWindow.windowCardPopupDrafting.Visibility = Visibility.Visible;

        }

        private void CardThumbnail_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindow.windowCardPopupDrafting.Visibility = Visibility.Hidden;
        }
    }
}
