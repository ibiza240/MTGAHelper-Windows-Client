using System.Windows;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardListPopup.xaml
    /// </summary>
    public partial class CardListPopup
    {
        public CardListPopup()
        {
            InitializeComponent();
        }

        public void Init(CardsListVM vm)
        {
            DataContext = vm;
        }

        public void SetCardPopupPosition(CardPopupSide side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            var popupWidth = (int)Width;

            int toLeft = mainWindowLeft - popupWidth;
            int toRight = mainWindowLeft + mainWindowWidth;

            int leftAdjusted = side switch
            {
                CardPopupSide.Left => toLeft,
                CardPopupSide.Right => toRight,
                _ => (mainWindowLeft < SystemParameters.WorkArea.Width / 2 ? toRight : toLeft)
            };

            Top = mainWindowTop;
            Left = leftAdjusted;

            CardsInPopup.SetCardPopupPosition(side, mainWindowTop, leftAdjusted, popupWidth);
        }
    }
}
