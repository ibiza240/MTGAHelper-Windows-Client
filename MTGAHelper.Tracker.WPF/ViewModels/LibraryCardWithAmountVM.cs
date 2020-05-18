using System.Windows.Media;
using MTGAHelper.Tracker.WPF.Models;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class LibraryCardWithAmountVM : CardWithAmountWpf
    {
        #region Public Properties

        /// <summary>
        /// Current card count string with name
        /// </summary>
        public string AmountAndName => $"{Amount}x {Name}";

        /// <summary>
        /// Fraction string of cards remaining in the deck and cards that started in the deck
        /// </summary>
        public string CardFraction => $"{Amount} / {_OriginalAmount}";

        /// <summary>
        /// Percentage chance to draw this card
        /// </summary>
        public float DrawPercent
        {
            get => _DrawPercent;
            set => SetField(ref _DrawPercent, value, nameof(DrawPercent));
        }

        /// <summary>
        /// Number of cards remaining in the deck
        /// </summary>
        public override int Amount
        {
            get => _Amount;
            set => IsAmountChanged = SetField(ref _Amount, value, nameof(Amount));
        }

        /// <summary>
        /// Has the amount of remaining cards changed? Used to animate the background color
        /// </summary>
        public bool IsAmountChanged
        {
            get => _IsAmountChanged;
            private set
            {
                SetField(ref _IsAmountChanged, value, nameof(IsAmountChanged));

                // Raise the property changed event for amount with original
                OnPropertyChanged(nameof(CardFraction));
            }
        }

        /// <summary>
        /// Border color based on card color
        /// </summary>
        public GradientStopCollection BorderGradient
        {
            get => _BorderGradient;
            set
            {
                SetField(ref _BorderGradient, value, nameof(BorderGradient));

                // Set the original value when the border is set because somebody hates constructors :(
                _OriginalAmount = Amount;
            }
        }

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// Percentage chance to draw this card
        /// </summary>
        private float _DrawPercent;

        /// <summary>
        /// Number of cards remaining in the deck
        /// </summary>
        private int _Amount;

        /// <summary>
        /// Has the amount of remaining cards changed? Used to animate the background color
        /// </summary>
        private bool _IsAmountChanged;

        /// <summary>
        /// Border color based on card color
        /// </summary>
        private GradientStopCollection _BorderGradient;

        #endregion

        #region Private Fields

        /// <summary>
        /// The number of cards the deck started with
        /// </summary>
        private int _OriginalAmount;

        #endregion
    }
}
