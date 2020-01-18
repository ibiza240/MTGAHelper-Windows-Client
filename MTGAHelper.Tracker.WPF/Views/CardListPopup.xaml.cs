using System.Windows;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardListPopup.xaml
    /// </summary>
    public partial class CardListPopup : Window
    {
        //CardsListVM vm = new CardsListVM(false);

        public CardListPopup()
        {
            //DataContext = vm;
            InitializeComponent();
            CardsInPopup.DisableRowStyleHighlight();
        }

        public void Init(CardsListVM vm)
        {
            DataContext = vm;
        }

        //internal void SetDataContext(string cardChosen, ICollection<CardWpf> cards)
        //{
        //    vm.SetCards(cardChosen, cards);
        //}

        public void SetCardPopupPosition(ForceCardPopupSideEnum side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            var popupWidth = (int)Width;

            var toLeft = mainWindowLeft - popupWidth;
            var toRight = mainWindowLeft + mainWindowWidth;

            var leftAdjusted = mainWindowLeft < SystemParameters.WorkArea.Width / 2 ? toRight : toLeft;
            if (side == ForceCardPopupSideEnum.Left)
                leftAdjusted = toLeft;
            else if (side == ForceCardPopupSideEnum.Right)
                leftAdjusted = toRight;

            Top = mainWindowTop;
            Left = leftAdjusted;

            CardsInPopup.SetCardPopupPosition(side, mainWindowTop, leftAdjusted, popupWidth);
        }
    }
}
