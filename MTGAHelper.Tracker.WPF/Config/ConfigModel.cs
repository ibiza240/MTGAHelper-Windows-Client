using System.IO;
using System.Threading;
using Newtonsoft.Json;
// All properties must have setters for reflection CopyProperties() method
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MTGAHelper.Tracker.WPF.Config
{
    public enum CardPopupSide
    {
        Auto,
        Left,
        Right,
    }

    public enum MinimizeOption
    {
        Taskbar,
        Tray,
        Height
    }

    public class Size
    {
        #region Constructors

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        public Size()
        {
            // For Serialization
        }

        /// <summary>
        /// Complete constructor
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Size(int w, int h)
        {
            Width = w;
            Height = h;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Width Value
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height Value
        /// </summary>
        public int Height { get; set; }

        #endregion
    }

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
        public Point Position { get; private set; } = new Point();

        /// <summary>
        /// The Window Size
        /// </summary>
        public Point Size { get; private set; } = new Point();

        /// <summary>
        /// The Window Transparency
        /// </summary>
        public double Opacity { get; set; } = 0.9;

        /// <summary>
        /// Whether the window is the topmost window
        /// </summary>
        public bool Topmost { get; set; } = true;

        /// <summary>
        /// Method for deep copying the window settings 
        /// </summary>
        /// <returns></returns>
        public WindowSettings Copy()
        {
            var ws = (WindowSettings)MemberwiseClone();
            ws.Position = new Point(Position.X, Position.Y);
            ws.Size = new Point(Size.X, Size.Y);
            return ws;
        }
    }

    public class ConfigModel
    {
        public string SigninProvider { get; set; }

        public string SigninEmail { get; set; }

        public string SigninPassword { get; set; }

        public string LogFilePath { get; set; } = "E.g. C:\\xyz\\Player.log";

        public string GameFilePath { get; set; }

        public string DraftHelperFolderCommunication { get; set; } = @"%AppData%\MTGAHelper\data\draftHelper";

        public bool RunOnStartup { get; set; }

        public bool AnimatedIcon { get; set; }

        public MinimizeOption Minimize { get; set; } = MinimizeOption.Taskbar;

        public bool AutoShowHideForMatch { get; set; }

        public CardPopupSide ForceCardPopupSide { get; set; } = CardPopupSide.Auto;

        public bool ShowLimitedRatings { get; set; } = true;

        public string LimitedRatingsSource { get; set; } = "Deathsie";

        public bool ShowOpponentCardsAuto { get; set; } = true;

        public bool ShowOpponentCardsExternal { get; set; } = true;

        public bool CardListCollapsed { get; set; }

        public string OrderLibraryCardsBy { get; set; } = "Converted Mana Cost";

        //public ConfigResolutionBlackBorders GameResolutionBlackBorders { get; set; }

        public bool ForceGameResolution { get; set; }

        public Size GameResolution { get; set; } = new Size(1920, 1080);

        public WindowSettings WindowSettings { get; set; } = new WindowSettings();

        public WindowSettings WindowSettingsOriginal { get; set; } = new WindowSettings();

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
