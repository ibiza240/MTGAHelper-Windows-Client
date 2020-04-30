using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SimpleCardsList.xaml
    /// </summary>
    public partial class SimpleCardsList
    {
        private readonly CardPopup WindowCardPopup = new CardPopup();

        public SimpleCardsList()
        {
            InitializeComponent();

        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is LibraryCardWithAmountVM vm)
                WindowCardPopup.ShowCard(vm.ImageCardUrl);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowCardPopup.Hide();
        }

        public void SetCardPopupPosition(ForceCardPopupSideEnum side, double mainWindowTop, double mainWindowLeft, double mainWindowWidth)
        {
            double popupWidth = WindowCardPopup.Width;

            double toLeft = mainWindowLeft - popupWidth;
            double toRight = mainWindowLeft + mainWindowWidth;

            double leftAdjusted = side switch
            {
                ForceCardPopupSideEnum.Left => toLeft,
                ForceCardPopupSideEnum.Right => toRight,
                _ => (mainWindowLeft < SystemParameters.WorkArea.Width / 2 ? toRight : toLeft)
            };

            WindowCardPopup.Top = mainWindowTop;
            WindowCardPopup.Left = leftAdjusted;
        }
    }
}
