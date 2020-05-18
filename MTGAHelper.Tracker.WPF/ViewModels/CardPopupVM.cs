using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardPopupVM: BasicModel
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
