using System;
using System.IO;
using Microsoft.Win32;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class MtgaResourcesLocator
    {
        #region Public Properties

        /// <summary>
        /// Action for setting or clearing a problem flag
        /// </summary>
        public Action<ProblemsFlags, bool> SetProblem { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Locate the log file path
        /// </summary>
        /// <param name="configApp"></param>
        public void LocateLogFilePath(ConfigModel configApp)
        {
            if (File.Exists(configApp.LogFilePath))
            {
                // File found
                SetProblem?.Invoke(ProblemsFlags.LogFileNotFound, false);
                return;
            }

            if (configApp.LogFilePath.StartsWith("E.g."))
            {
                string pathDir =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/Wizards Of The Coast/MTGA";
                if (Directory.Exists(pathDir))
                {
                    // File deduced
                    configApp.LogFilePath = Path.GetFullPath($"{pathDir}/Player.log");
                    configApp.Save();
                    SetProblem?.Invoke(ProblemsFlags.LogFileNotFound, false);
                    return;
                }
            }

            // File not found
            SetProblem?.Invoke(ProblemsFlags.LogFileNotFound, true);
        }

        /// <summary>
        /// Locate the game file path
        /// </summary>
        /// <param name="configApp"></param>
        public void LocateGameClientFilePath(ConfigModel configApp)
        {
            if (File.Exists(configApp.GameFilePath))
            {
                // File found
                SetProblem?.Invoke(ProblemsFlags.GameClientFileNotFound, false);
                return;
            }

            if (string.IsNullOrWhiteSpace(configApp.GameFilePath))
            {
                using RegistryKey key =
                    Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Wizards of the Coast\\MTGArena");

                if (key?.GetValue("Path") is string s)
                {
                    // File deduced
                    configApp.GameFilePath = Path.Combine(s, "MTGA.exe");
                    configApp.Save();
                    SetProblem?.Invoke(ProblemsFlags.GameClientFileNotFound, false);
                    return;
                }
            }

            // File not found
            SetProblem?.Invoke(ProblemsFlags.GameClientFileNotFound, true);
        }

        #endregion
    }
}
