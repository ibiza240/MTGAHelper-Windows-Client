using IWshRuntimeLibrary;
using System;
using System.IO;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class StartupShortcutManager
    {
        public bool ManageRunOnStartup(bool runOnStartup)
        {
            //try
            //{
            var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "MTGAHelper Tracker.lnk");
            var shortcutExists = System.IO.File.Exists(shortcutPath);

            if (runOnStartup)
            {
                if (shortcutExists == false)
                    CreateShortcut(shortcutPath);
            }
            else if (shortcutExists)
            {
                System.IO.File.Delete(shortcutPath);
            }

            return true;
            //}
            //catch (Exception ex)
            //{
            //    Log.Write(Serilog.Events.LogEventLevel.Error, ex, "Could not change the startup config:");
            //    return false;
            //}
        }

        void CreateShortcut(string shortcutPath)
        {
            var appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");

            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(shortcutPath) as IWshShortcut;
            shortcut.TargetPath = appLocation;
            shortcut.WorkingDirectory = Path.GetDirectoryName(appLocation);
            shortcut.Save();
        }
    }
}
