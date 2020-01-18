namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OptionsWindowVM
    {
        //public string UserId { get; set; } = "Test";
        public string LogFilePath { get; set; } = "Test";
        public string GameFilePath { get; set; } = "Test";
        public bool RunOnStartup { get; set; } = true;
        public bool MinimizeToSystemTray { get; set; } = false;
        public bool AutoShowHideForMatch { get; set; }
        public bool ShowOpponentCards { get; set; } = true;

        public bool ForceCardPopup { get; set; } = false;
        public string ForceCardPopupSide { get; set; } = "On the left";
        public string[] ForceCardPopupSides { get; set; } = new string[]
        {
            "On the left",
            "On the right",
        };

        public bool ShowLimitedRatings { get; set; } = true;
        public string ShowLimitedRatingsSource { get; set; } = "ChannelFireball (LSV)";
        public string[] ShowLimitedRatingsSources { get; set; } = new string[0];
        //{
        //    "ChannelFireball (LSV)",
        //    "DraftSim",
        //    "Deathsie",
        //    "Mtg Community Review",
        //};

        public string OrderLibraryCardsBy { get; set; } = "Converted Mana Cost";
        public string[] OrderLibraryCardsByItems { get; set; } = new string[]
        {
            "Converted Mana Cost",
            "Highest Draw % chance",
        };
    }
}
