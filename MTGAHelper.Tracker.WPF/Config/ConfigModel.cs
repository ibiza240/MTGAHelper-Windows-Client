using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace MTGAHelper.Tracker.WPF.Config
{
    public class Point
    {
        #region Constructors

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        public Point()
        {
            // For Serialization
        }

        /// <summary>
        /// Complete constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// X Value
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y Value
        /// </summary>
        public double Y { get; set; }

        #endregion
    }

    public class WindowSettings
    {
        /// <summary>
        /// The Window Position
        /// </summary>
        public Point Position { get; } = new Point();

        /// <summary>
        /// The Window Size
        /// </summary>
        public Point Size { get; } = new Point();

        /// <summary>
        /// The Window Transparency
        /// </summary>
        public double Opacity { get; set; } = 0.9;

        /// <summary>
        /// Whether the window is the topmost window
        /// </summary>
        public bool Topmost { get; set; } = true;
    }

    public class ConfigModel
    {
        public string SigninProvider { get; set; }

        public string SigninEmail { get; set; }

        public string SigninPassword { get; set; }

        public string LogFilePath { get; set; } = "E.g. C:\\xyz\\output_log.txt";

        public string GameFilePath { get; set; }

        public string DraftHelperFolder { get; set; } = @".\DraftHelper";

        public string DraftHelperFolderCommunication { get; set; } = @"%AppData%\MTGAHelper\data\draftHelper";

        public bool RunOnStartup { get; set; }

        public bool AnimatedIcon { get; set; }

        public bool MinimizeToSystemTray { get; set; }

        public bool AutoShowHideForMatch { get; set; }

        public bool ForceCardPopup { get; set; }

        public string ForceCardPopupSide { get; set; } = "On the left";

        public bool ShowLimitedRatings { get; set; } = true;

        public string ShowLimitedRatingsSource { get; set; } = "Deathsie";

        public bool ShowOpponentCardsAuto { get; set; } = true;

        public bool ShowOpponentCardsExternal { get; set; } = true;

        public string OrderLibraryCardsBy { get; set; } = "Converted Mana Cost";

        public WindowSettings WindowSettings { get; set; } = new WindowSettings();

        public WindowSettings WindowSettingsOpponentCards { get; set; } = new WindowSettings();

        internal void Save()
        {
            string configFolder = new DebugOrRelease().GetConfigFolder();
            string configFile = Path.Combine(configFolder, "appsettings.json");
            var saved = false;
            while (saved == false)
            {
                //var configFile = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(this));

                // Safety check in case of invalid file saved
                long fileSize = new FileInfo(configFile).Length;
                saved = fileSize > 0;
                if (saved == false)
                    Thread.Sleep(1000);
            }
        }
    }
}
