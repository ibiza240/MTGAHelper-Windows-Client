using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Config
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
            // For Serialization
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class WindowSettings
    {
        public Point Position { get; set; } = new Point();
        public Point Size { get; set; } = new Point();
    }

    public class ConfigModelApp
    {
        //public string UserId { get; set; }
        public string SigninProvider { get; set; }
        public string SigninEmail { get; set; }
        public string SigninPassword { get; set; }
        public string LogFilePath { get; set; } = "E.g. C:\\xyz\\output_log.txt";
        public string GameFilePath { get; set; }
        public bool RunOnStartup { get; set; }
        public bool AlwaysOnTop { get; set; } = true;
        public bool MinimizeToSystemTray { get; set; }
        public bool AutoShowHideForMatch { get; set; }
        public int Opacity { get; set; } = 90;

        public bool Test { get; set; }

        public bool ForceCardPopup { get; set; }
        public string ForceCardPopupSide { get; set; } = "On the left";

        public bool ShowLimitedRatings { get; set; } = true;
        public string ShowLimitedRatingsSource { get; set; } = "ChannelFireball (LSV)";

        public WindowSettings WindowSettings { get; set; } = new WindowSettings();
        public WindowSettings WindowSettingsOpponentCards { get; set; } = new WindowSettings();

        internal void Save()
        {
#if DEBUG || DEBUGWITHSERVER
            var configFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#else
            var configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
#endif
            var configFile = Path.Combine(configFolder, "appsettings.json");
            //var configFile = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            File.WriteAllText(configFile, JsonConvert.SerializeObject(this));
        }
    }
}
