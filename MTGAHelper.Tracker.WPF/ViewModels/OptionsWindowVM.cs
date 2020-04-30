namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OptionsWindowVM
    {
        public string LogFilePath { get; set; } = "Test";

        public string GameFilePath { get; set; } = "Test";

        public bool RunOnStartup { get; set; } = true;

        public bool MinimizeToSystemTray { get; set; }

        public bool AutoShowHideForMatch { get; set; }

        public bool ShowOpponentCardsAuto { get; set; } = true;

        public bool ShowOpponentCardsExternal { get; set; } = true;

        public bool ForceCardPopup { get; set; }

        public string ForceCardPopupSide { get; set; } = "On the left";

        public string[] ForceCardPopupSides { get; } =
        {
            "On the left",
            "On the right",
        };

        public bool ShowLimitedRatings { get; set; } = true;

        public string ShowLimitedRatingsSource { get; set; } = "Deathsie";

        public string[] ShowLimitedRatingsSources { get; set; } = new string[0];

        public string OrderLibraryCardsBy { get; set; } = "Converted Mana Cost";

        public string[] OrderLibraryCardsByItems { get; } =
        {
            "Converted Mana Cost",
            "Highest Draw % chance",
        };
    }
}
