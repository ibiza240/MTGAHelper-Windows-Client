using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Config
{
    public class ConfigModelApp
    {
        public string UserId { get; set; }
        public string LogFilePath { get; set; }
        public string GameFilePath { get; set; }
        public bool RunOnStartup { get; set; }
        public bool SkipVersionCheck { get; set; }

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
