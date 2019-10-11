using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OptionsWindowVM
    {
        public string UserId { get; set; } = "Test";
        public string LogFilePath { get; set; } = "Test";
        public string GameFilePath { get; set; } = "Test";
        public bool RunOnStartup { get; set; } = true;
        public bool MinimizeToSystemTray { get; set; } = false;
    }
}
