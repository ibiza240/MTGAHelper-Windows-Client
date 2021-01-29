using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OptionsWindowVM : BasicModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public OptionsWindowVM()
        {
            // Subscribe to property changes
            PropertyChanged += OnPropertyChanged;
            LogFile.PropertyChanged += LogFile_PropertyChanged;
            GameFile.PropertyChanged += GameFile_PropertyChanged;
        }

        #endregion Constructor

        #region ConfigurablePaths

        /// <summary>
        /// Path to the log file
        /// </summary>
        public ConfigurablePath LogFile { get; } =
            new ConfigurablePath(ConfigurablePath.BrowseType.OpenFile, "")
            {
                DefaultDisplayName = "Log File",
                FileFilter = "Log Files|*.log|All Files|*.*"
            };

        /// <summary>
        /// Path the the game executable
        /// </summary>
        public ConfigurablePath GameFile { get; } =
            new ConfigurablePath(ConfigurablePath.BrowseType.OpenFile, "")
            {
                DefaultDisplayName = "MTG Arena Executable",
                FileFilter = "Executables|*.exe|All Files|*.*"
            };

        #endregion ConfigurablePaths

        #region Two Way Bindable Properties

        /// <summary>
        /// Path to the log file
        /// </summary>
        public string LogFilePath
        {
            get => _LogFilePath;
            set => SetField(ref _LogFilePath, value, nameof(LogFilePath));
        }

        /// <summary>
        /// Path to the game executable
        /// </summary>
        public string GameFilePath
        {
            get => _GameFilePath;
            set => SetField(ref _GameFilePath, value, nameof(GameFilePath));
        }

        /// <summary>
        /// Whether the tracker should run on Windows startup
        /// </summary>
        public bool RunOnStartup
        {
            get => _RunOnStartup;
            set => SetField(ref _RunOnStartup, value, nameof(RunOnStartup));
        }

        /// <summary>
        /// Minimize window option
        /// </summary>
        public MinimizeOption Minimize
        {
            get => _Minimize;
            set => SetField(ref _Minimize, value, nameof(Minimize));
        }

        /// <summary>
        /// Whether to minimize and restore the window between games
        /// </summary>
        public bool AutoShowHideForMatch
        {
            get => _AutoShowHideForMatch;
            set => SetField(ref _AutoShowHideForMatch, value, nameof(AutoShowHideForMatch));
        }

        /// <summary>
        /// Whether to show the opponent cards automatically
        /// </summary>
        public bool ShowOpponentCardsAuto
        {
            get => _ShowOpponentCardsAuto;
            set => SetField(ref _ShowOpponentCardsAuto, value, nameof(ShowOpponentCardsAuto));
        }

        /// <summary>
        /// Whether to show the opponent cards in an external window
        /// </summary>
        public bool ShowOpponentCardsExternal
        {
            get => _ShowOpponentCardsExternal;
            set => SetField(ref _ShowOpponentCardsExternal, value, nameof(ShowOpponentCardsExternal));
        }

        public bool ShowAllDraftRatings
        {
            get => _ShowAllDraftRatings;
            set => SetField(ref _ShowAllDraftRatings, value, nameof(ShowAllDraftRatings));
        }

        /// <summary>
        /// Whether to force the resolution for draft helper
        /// </summary>
        public bool ForceGameResolution
        {
            get => _ForceGameResolution;
            set => SetField(ref _ForceGameResolution, value, nameof(ForceGameResolution));
        }

        /// <summary>
        /// Which side to force the popup
        /// </summary>
        public CardPopupSide ForceCardPopupSide
        {
            get => _ForceCardPopupSide;
            set => SetField(ref _ForceCardPopupSide, value, nameof(ForceCardPopupSide));
        }

        /// <summary>
        /// Whether to show the limited rating
        /// </summary>
        public bool ShowLimitedRatings
        {
            get => _ShowLimitedRatings;
            set => SetField(ref _ShowLimitedRatings, value, nameof(ShowLimitedRatings));
        }

        /// <summary>
        /// The selected limited ratings source
        /// </summary>
        public KeyValuePair<string, string> LimitedRatingsSource
        {
            get => _LimitedRatingsSource;
            set => SetField(ref _LimitedRatingsSource, value, nameof(LimitedRatingsSource));
        }

        /// <summary>
        /// How to order the card list
        /// </summary>
        public string OrderLibraryCardsBy
        {
            get => _OrderLibraryCardsBy;
            set => SetField(ref _OrderLibraryCardsBy, value, nameof(OrderLibraryCardsBy));
        }

        /// <summary>
        /// Game resolution override for draft helper
        /// </summary>
        public Size GameResolution
        {
            get => _GameResolution;
            set => SetField(ref _GameResolution, value, nameof(GameResolution));
        }

        /// <summary>
        /// Whether the game resolution is panoramic
        /// </summary>
        //public ConfigResolutionBlackBorders GameResolutionBlackBorders
        //{
        //    get => _GameResolutionBlackBorders;
        //    set => SetField(ref _GameResolutionBlackBorders, value, nameof(GameResolutionBlackBorders));
        //}

        #endregion Two Way Bindable Properties

        #region Drop Down Options

        /// <summary>
        /// Minimize window enumeration options
        /// </summary>
        public Array MinimizeOptions => Enum.GetValues(typeof(MinimizeOption));

        /// <summary>
        /// Options for limited ratings sources
        /// </summary>
        //public IEnumerable<string> LimitedRatingsSources => DraftRatings.Get().Keys.OrderBy(i => i).Append(Constants.LIMITEDRATINGS_SOURCE_CUSTOM).ToArray();

        public Dictionary<string, string> LimitedRatingsSourcesDict => DraftRatings.Get().Keys
            .OrderBy(i => i)
            .Append(Constants.LIMITEDRATINGS_SOURCE_CUSTOM)
            .ToDictionary(set => set, set => set + (set == Constants.LIMITEDRATINGS_SOURCE_CUSTOM ? "" : $" [{string.Join(", ", DraftRatings.Get()[set].RatingsBySet.Keys.OrderByDescending(i => Sets[i].ReleaseDate))}]"));

        /// <summary>
        /// Options for forcing the card popup side
        /// </summary>
        public Array ForceCardPopupSides => Enum.GetValues(typeof(CardPopupSide));

        /// <summary>
        /// Options for ordering the card list
        /// </summary>
        public IEnumerable<string> OrderLibraryCardsByItems { get; } =
            new[] { "Converted Mana Cost", "Highest Draw % chance" };

        #endregion Drop Down Options

        #region Public Properties

        /// <summary>
        /// DraftRatings for accessing sources
        /// </summary>
        public CacheSingleton<Dictionary<string, DraftRatings>> DraftRatings { get; set; }

        public Dictionary<string, Set> Sets { get; internal set; }

        #endregion Public Properties

        #region Private Backing Fields

        /// <summary>
        /// Path to the log file
        /// </summary>
        private string _LogFilePath;

        /// <summary>
        /// Path to the game executable
        /// </summary>
        private string _GameFilePath;

        /// <summary>
        /// Whether the tracker should run on Windows startup
        /// </summary>
        private bool _RunOnStartup;

        /// <summary>
        /// Minimize window option
        /// </summary>
        private MinimizeOption _Minimize;

        /// <summary>
        /// Whether to minimize and restore the window between games
        /// </summary>
        private bool _AutoShowHideForMatch;

        /// <summary>
        /// Whether to show the opponent cards automatically
        /// </summary>
        private bool _ShowOpponentCardsAuto;

        /// <summary>
        /// Whether to show the opponent cards in an external window
        /// </summary>
        private bool _ShowOpponentCardsExternal;

        private bool _ShowAllDraftRatings;

        /// <summary>
        /// Whether to force the resolution for draft helper
        /// </summary>
        private bool _ForceGameResolution;

        /// <summary>
        /// Which side to force the popup
        /// </summary>
        private CardPopupSide _ForceCardPopupSide;

        /// <summary>
        /// Whether to show the limited rating
        /// </summary>
        private bool _ShowLimitedRatings;

        /// <summary>
        /// The selected limited ratings source
        /// </summary>
        private KeyValuePair<string, string> _LimitedRatingsSource;

        /// <summary>
        /// How to order the card list
        /// </summary>
        private string _OrderLibraryCardsBy;

        /// <summary>
        /// Game resolution override for draft helper
        /// </summary>
        private Size _GameResolution;

        /// <summary>
        /// Whether the game resolution is panoramic
        /// </summary>
        //private ConfigResolutionBlackBorders _GameResolutionBlackBorders;

        #endregion Private Backing Fields

        #region Private Methods

        /// <summary>
        /// Handles changes to the local variables (from the AutoMapper)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LogFilePath):
                    {
                        LogFile.ModelValue = LogFilePath;
                        break;
                    }
                case nameof(GameFilePath):
                    {
                        GameFile.ModelValue = GameFilePath;
                        break;
                    }
            }
        }

        /// <summary>
        /// Handle changes to the game file browse paths
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigurablePath.ViewValueString):
                    {
                        GameFilePath = GameFile.ViewValueString;
                        break;
                    }
            }
        }

        /// <summary>
        /// Handle changes to the log file browse paths
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigurablePath.ViewValueString):
                    {
                        LogFilePath = LogFile.ViewValueString;
                        break;
                    }
            }
        }

        #endregion Private Methods
    }
}
