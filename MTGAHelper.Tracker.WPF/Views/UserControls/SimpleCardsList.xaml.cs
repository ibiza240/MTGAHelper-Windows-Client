using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SimpleCardsList.xaml
    /// </summary>
    public partial class SimpleCardsList : UserControl
    {
        readonly CardPopup windowCardPopup = new CardPopup();

        public SimpleCardsList()
        {
            InitializeComponent();

        }

        public void DisableRowStyleHighlight()
        {
            gridCards.RowStyle = new Style(typeof(DataGridRow));
            gridCards.RowStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Color.FromArgb(0, 0x27, 0x2b, 0x30))));
        }

        //public void SetDataContext(CardsListVM dataContext)
        //{
        //    DataContext = dataContext;
        //}

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as LibraryCardWithAmountVM;
            windowCardPopup.ShowCard(vm.ImageCardUrl);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            windowCardPopup.Hide();
        }

        public void SetCardPopupPosition(ForceCardPopupSideEnum side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            //var width = (int)windowCardPopup.Width;
            //var leftAdjusted = left < SystemParameters.WorkArea.Width / 2 ? left + width : left - width;
            var popupWidth = (int)windowCardPopup.Width;

            var toLeft = mainWindowLeft - popupWidth;
            var toRight = mainWindowLeft + mainWindowWidth;

            var leftAdjusted = mainWindowLeft < SystemParameters.WorkArea.Width / 2 ? toRight : toLeft;
            if (side == ForceCardPopupSideEnum.Left)
                leftAdjusted = toLeft;
            else if (side == ForceCardPopupSideEnum.Right)
                leftAdjusted = toRight;

            windowCardPopup.Top = mainWindowTop;
            windowCardPopup.Left = leftAdjusted;
        }

        private void gridCards_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
