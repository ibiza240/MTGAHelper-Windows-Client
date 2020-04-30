namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardPopupVM: ObservableObject
    {
        /// <summary>
        /// Card Image URL
        /// </summary>
        public string ImageCardUrl
        {
            get => _ImageCardUrl;
            set => SetField(ref _ImageCardUrl, value, nameof(ImageCardUrl));
        }

        /// <summary>
        /// Card Image URL
        /// </summary>
        private string _ImageCardUrl;
    }
}
