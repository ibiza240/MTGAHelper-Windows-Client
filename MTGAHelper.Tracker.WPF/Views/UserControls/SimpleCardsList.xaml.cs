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
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SimpleCardsList.xaml
    /// </summary>
    public partial class SimpleCardsList : UserControl
    {
        CardPopup windowCardPopup = new CardPopup();

        public SimpleCardsList()
        {
            InitializeComponent();
        }

        public void SetDataContext(CardsListVM dataContext)
        {
            DataContext = dataContext;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as LibraryCardWithAmountVM;
            windowCardPopup.ShowCard(vm.ImageCardUrl);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            windowCardPopup.Hide();
        }

        public void SetCardPopupPosition(int top, int left)
        {
            var width = (int)windowCardPopup.Width;
            var leftAdjusted = left < SystemParameters.WorkArea.Width / 2 ? left + width : left - width;
            windowCardPopup.Top = top;
            windowCardPopup.Left = leftAdjusted;
        }
    }
}
