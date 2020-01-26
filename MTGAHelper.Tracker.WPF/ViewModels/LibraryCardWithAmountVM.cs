using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using MTGAHelper.Tracker.WPF.Models;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class LibraryCardWithAmountVM : CardWithAmountWpf, INotifyPropertyChanged
    {
        GradientStopCollection borderGradient;
        public GradientStopCollection BorderGradient
        {
            get => borderGradient;
            set
            {
                borderGradient = value;
                originalAmount = Amount;
            }
        }

        float drawPercent;

        public float DrawPercent
        {
            get => drawPercent;
            set
            {
                if (drawPercent == value) return;
                drawPercent = value;
                OnPropertyChanged();
            }
        }

        public string AmountAndName => $"{Amount}x {Name}";

        public string AmountWithOriginal => $"{Amount}/{originalAmount}";

        int originalAmount;

        int amount;

        public override int Amount
        {
            get => amount;
            set
            {
                if (amount == value)
                {
                    IsAmountChanged = false;
                    return;
                }
                amount = value;
                OnPropertyChanged(string.Empty);
                IsAmountChanged = true;
            }
        }

        bool isAmountChanged;
        public bool IsAmountChanged
        {
            get => isAmountChanged;
            private set { isAmountChanged = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
