using System;
using System.IO;
using Microsoft.Win32;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class MtgaResourcesLocator
    {
        readonly MainWindowVM vm;

        public MtgaResourcesLocator(MainWindowVM vm)
        {
            this.vm = vm;
        }

        public void LocateLogFilePath(ConfigModelApp configApp)
        {
            if (File.Exists(configApp.LogFilePath))
            {
                // File found
                vm.SetProblem(ProblemsFlags.LogFileNotFound, false);
                return;
            }
            else if (configApp.LogFilePath.StartsWith("E.g."))
            {
                var pathDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/Wizards Of The Coast/MTGA";
                if (Directory.Exists(pathDir))
                {
                    // File deduced
                    configApp.LogFilePath = Path.GetFullPath($"{pathDir}/output_log.txt");
                    configApp.Save();
                    vm.SetProblem(ProblemsFlags.LogFileNotFound, false);
                    return;
                }
            }

            // File not found
            vm.SetProblem(ProblemsFlags.LogFileNotFound, true);
        }

        public void LocateGameClientFilePath(ConfigModelApp configApp)
        {
            if (File.Exists(configApp.GameFilePath))
            {
                // File found
                vm.SetProblem(ProblemsFlags.GameClientFileNotFound, false);
                return;
            }
            else if (string.IsNullOrWhiteSpace(configApp.GameFilePath))
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Wizards of the Coast\\MTGArena"))
                {
                    if (key != null)
                    {
                        var o = key.GetValue("Path");
                        if (o != null)
                        {
                            // File deduced
                            configApp.GameFilePath = Path.Combine(o as string, "MTGA.exe");
                            configApp.Save();
                            vm.SetProblem(ProblemsFlags.GameClientFileNotFound, false);
                            return;
                        }
                    }
                }
            }

            // File not found
            vm.SetProblem(ProblemsFlags.GameClientFileNotFound, true);
        }
    }
}
