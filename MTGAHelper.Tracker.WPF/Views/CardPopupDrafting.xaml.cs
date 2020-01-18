using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views
{
    public enum ForceCardPopupSideEnum
    {
        None,
        Left,
        Right,
    }

    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopupDrafting : Window
    {
        readonly DraftingCardPopupVM vm = new DraftingCardPopupVM();

        public CardPopupDrafting()
        {
            //vm = (GlobalVM)DataContext;
            DataContext = vm;

            InitializeComponent();

            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(200);
            //timer.Tick += (object sender, EventArgs e) =>
            //{
            //    Top = vm.CardPopupTop;
            //    Left = vm.CardPopupLeft;
            //};
            //timer.Start();
        }

        public void Refresh(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays)
        {
            //this.vm.CardPopupImageUrl.Value = vm.CardPopupImageUrl.Value;
            //this.vm.Description.Value = vm.Description.Value;
            //this.vm.Rating.Value = vm.Rating.Value;
            //this.vm.TopCommonCard.Value = vm.TopCommonCard.Value;
            this.vm.SetDraftCard(cardVM, showGlobalMTGAHelperSays);
        }

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
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            vm.SetPopupRatingsSource(showRatingsSource, source);
        }
    }
}
