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

            // Keep the appsettings.json
            var keepAppSettings = File.Exists(fileAppSettings);
            if (keepAppSettings)
                File.Copy(fileAppSettings, fileAppSettingsCopy, true);

            Console.WriteLine("Installing the latest version...");
            Process pInstall = new Process();
            var folderInstalled = GetFolderInstalled("MTGAHelper.Tracker.dll");
            var processFileName = localFilepath;
            if (folderInstalled == null)
            {
                Console.WriteLine("No previous version was found installed");
                pInstall.StartInfo.UseShellExecute = true;
                pInstall.StartInfo.FileName = processFileName;
            }
            else
            {
                Console.WriteLine($"Upgrading the version found at '{folderInstalled}'");
                pInstall.StartInfo.FileName = "msiexec";
                pInstall.StartInfo.Arguments = $"/i \"{localFilepath}\" /passive TARGETDIR=\"{folderInstalled}\"";
                
            }
            pInstall.Start();
            pInstall.WaitForExit();

            // Keep the appsettings.json settings
            if (keepAppSettings)
                KeepAppSettings(fileAppSettingsCopy, fileAppSettings);

            Console.WriteLine("Installation complete! Enjoy the latest version of MTGAHelper Tracker :)");

            var pathAppLink = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTGAHelper Tracker.lnk");
            if (File.Exists(pathAppLink))
            {
                Console.WriteLine("Launching the program...");
                Process pTracker = new Process();
                pTracker.StartInfo.FileName = pathAppLink;
                pTracker.StartInfo.UseShellExecute = true;
                pTracker.Start();
            }

            Thread.Sleep(1000);
        }

        private static void KeepAppSettings(string fileAppSettingsCopy, string fileAppSettings)
        {
            var oldSettings = JObject.Parse(File.ReadAllText(fileAppSettingsCopy));
            var newSettings = JObject.Parse(File.ReadAllText(fileAppSettings));
            foreach (var keyValue in oldSettings)
            {
                newSettings[keyValue.Key] = keyValue.Value;
            }

            File.WriteAllText(fileAppSettings, JsonConvert.SerializeObject(newSettings));
        }

        private static void DownloadLatest(string localFilepath)
        {
            using (WebClient c = new WebClient())
            {
                c.DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelperTracker.msi", localFilepath);
            }
        }

        static string GetFolderInstalled(string file)
        {
            var keyFolder = @"SOFTWARE\Microsoft\Installer\Assemblies";
            var key = Registry.CurrentUser.OpenSubKey(keyFolder);

            if (key == null)
                return null;

            var dllKey = key.GetSubKeyNames()
                .Select(keyName => key.OpenSubKey(keyName))
                .FirstOrDefault(i => i.Name.Contains(file));

            if (dllKey == null)
                return null;

            var filePath = dllKey.Name.Replace(@$"HKEY_CURRENT_USER\{keyFolder}\", "").Replace("|", @"\");
            var installationFolder = Path.GetDirectoryName(filePath);
            return installationFolder;
        }
    }
}
