namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardPopupVM
    {
        public ObservableProperty<string> ImageCardUrl { get; set; } = new ObservableProperty<string>(null);
    }
}
