using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class LibraryCardWithAmountVM : CardWithAmountWpf
    {
        public delegate void CardNotificationHandler(object sender, EventArgs e);

        public CardVM CardVM { get; set; }

        public ObservableProperty<float> DrawPercent { get; set; } = new ObservableProperty<float>(0f);

        public ObservableProperty<string> AmountAndName { get; set; } = new ObservableProperty<string>("");

        public ObservableProperty<string> AmountWithOriginal { get; set; } = new ObservableProperty<string>("");

        public ObservableProperty<int> AmountHackishObservable { get; set; } = new ObservableProperty<int>(0);

        internal void RefreshBindings(int? originalAmount)
        {
            CardVM.SetColorBorder();
            AmountHackishObservable.Value = Amount;
            AmountAndName.Value = $"{Amount}x {Name}";
            AmountWithOriginal.Value = $"{Amount}{(originalAmount.HasValue ? "/" + originalAmount.Value : "")}";
            
        }
    }
}
