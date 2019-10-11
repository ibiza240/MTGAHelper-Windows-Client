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
        public Point Position { get; set; }
        public Point Size { get; set; }
    }

    public class ConfigModelApp
    {
        public string UserId { get; set; }
        public string LogFilePath { get; set; }
        public string GameFilePath { get; set; }
        public bool RunOnStartup { get; set; } = true;
        public bool AlwaysOnTop { get; set; } = true;
        public bool MinimizeToSystemTray { get; set; } = false;
        public int Opacity { get; set; } = 90;

        public bool Test { get; set; }

        public WindowSettings WindowSettings { get; set; }

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
