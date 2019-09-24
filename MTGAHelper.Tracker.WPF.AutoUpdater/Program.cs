using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTGAHelper.Tracker.WPF.AutoUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper", "appsettings.json");
            var fileAppSettingsCopy = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper", "appsettings.json.bak");

            Console.WriteLine("Welcome to the MTGAHelper Tracker Auto-updater. This will remove the current version and install the latest one.");
            Console.WriteLine("----------------------------------------");

            Console.WriteLine("Downloading the latest version...");
            var localFilepath = Path.Combine(Path.GetTempPath(), "MTGAHelperTracker.msi");
            DownloadLatest(localFilepath);
            Console.WriteLine($"Download complete ({localFilepath})");
            Console.WriteLine("----------------------------------------");

            if (IsSoftwareInstalled("MTGAHelper Tracker"))
            {
                Console.WriteLine("Uninstalling the current version...");
                // Keep the appsettings.json
                File.Copy(fileAppSettings, fileAppSettingsCopy, true);
                Process pUninstall = new Process();
                pUninstall.StartInfo.FileName = "msiexec.exe";
                pUninstall.StartInfo.Arguments = "/x {36D1B5B8-2ED5-4588-A127-B9122E1E4D3A}";
                pUninstall.Start();
                pUninstall.WaitForExit();
                Console.WriteLine("MTGAHelper has been uninstalled");
                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("Installing the latest version...");
            Process pInstall = new Process();
            pInstall.StartInfo.FileName = localFilepath;
            pInstall.StartInfo.UseShellExecute = true;
            pInstall.Start();
            pInstall.WaitForExit();
            // Keep the appsettings.json settings
            KeepAppSettings(fileAppSettingsCopy, fileAppSettings);
            Console.WriteLine("Installation complete! Enjoy the latest version of MTGAHelper Tracker :)");

            Thread.Sleep(1000);

            Process pTracker = new Process();
            pTracker.StartInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTGAHelper Tracker.lnk");
            pTracker.StartInfo.UseShellExecute = true;
            pTracker.Start();
        }

        private static void KeepAppSettings(string fileAppSettingsCopy, string fileAppSettings)
        {
            //File.Copy(fileAppSettingsCopy, fileAppSettings, true);
            //File.Delete(fileAppSettingsCopy);

            var oldSettings = JObject.Parse(File.ReadAllText(fileAppSettingsCopy));
            var newSettings = JObject.Parse(File.ReadAllText(fileAppSettings));
            foreach (var keyValue in oldSettings)
            {
                newSettings[keyValue.Key] = keyValue.Value;
            }

            File.WriteAllText(fileAppSettings, JsonConvert.SerializeObject(newSettings));
            File.Delete(fileAppSettingsCopy);
        }

        private static void DownloadLatest(string localFilepath)
        {
            using (WebClient c = new WebClient())
            {
                c.DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelperTracker.msi", localFilepath);
            }
        }

        static bool IsSoftwareInstalled(string softwareName)
        {
            var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall") ?? Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            if (key == null)
                return false;

            return key.GetSubKeyNames()
                .Select(keyName => key.OpenSubKey(keyName))
                .Select(subkey => subkey.GetValue("DisplayName") as string)
                .Any(displayName => displayName != null && displayName.Contains(softwareName));
        }
    }
}
