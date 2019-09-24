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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopupDrafting : Window
    {
        DraftingCardPopupVM vm = new DraftingCardPopupVM();

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

        public void SetCardPopupPosition(int top, int left, int width)
        {
            var leftAdjusted = left < SystemParameters.WorkArea.Width / 2 ? left + Width : left - width;
            Top = top;
            Left = leftAdjusted;
        }
    }
}
