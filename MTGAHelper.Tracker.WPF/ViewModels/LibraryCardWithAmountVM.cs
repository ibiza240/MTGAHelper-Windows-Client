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

        public override int Amount
        {
            get => base.Amount;
            set
            {
                if (base.Amount == value) return;
                base.Amount = value;
                OnPropertyChanged(string.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
