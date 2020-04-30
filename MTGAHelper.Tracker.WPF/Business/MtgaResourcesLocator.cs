using System;
using System.IO;
using Microsoft.Win32;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class MtgaResourcesLocator
    {
        private readonly MainWindowVM VM;

        public MtgaResourcesLocator(MainWindowVM vm)
        {
            VM = vm;
        }

        public void LocateLogFilePath(ConfigModel configApp)
        {
            if (File.Exists(configApp.LogFilePath))
            {
                // File found
                VM.UnSetProblem(ProblemsFlags.LogFileNotFound);
                return;
            }

            if (configApp.LogFilePath.StartsWith("E.g."))
            {
                string pathDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/Wizards Of The Coast/MTGA";
                if (Directory.Exists(pathDir))
                {
                    // File deduced
                    configApp.LogFilePath = Path.GetFullPath($"{pathDir}/output_log.txt");
                    configApp.Save();
                    VM.UnSetProblem(ProblemsFlags.LogFileNotFound);
                    return;
                }
            }

            // File not found
            VM.SetProblem(ProblemsFlags.LogFileNotFound);
        }

        public void LocateGameClientFilePath(ConfigModel configApp)
        {
            if (File.Exists(configApp.GameFilePath))
            {
                // File found
                VM.UnSetProblem(ProblemsFlags.GameClientFileNotFound);
                return;
            }
            else if (string.IsNullOrWhiteSpace(configApp.GameFilePath))
            {
                using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Wizards of the Coast\\MTGArena"))
                {
                    if (key != null)
                    {
                        var o = key.GetValue("Path");
                        if (o != null)
                        {
                            // File deduced
                            configApp.GameFilePath = Path.Combine(o as string, "MTGA.exe");
                            configApp.Save();
                            VM.UnSetProblem(ProblemsFlags.GameClientFileNotFound);
                            return;
                        }
                    }
                }
            }

            // File not found
            VM.SetProblem(ProblemsFlags.GameClientFileNotFound);
        }
    }
}
