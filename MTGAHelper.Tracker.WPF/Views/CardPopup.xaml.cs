using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopup
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CardPopup()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Window data context and view model reference
        /// </summary>
        private readonly CardPopupVM ViewModel = new CardPopupVM();

        #endregion

        #region Internal Methods

        /// <summary>
        /// Set the Image URL and show the window
        /// </summary>
        /// <param name="imageCardUrl"></param>
        internal void ShowCard(string imageCardUrl)
        {
            ViewModel.ImageCardUrl = imageCardUrl;

            Show();
        }

        #endregion
    }
}
