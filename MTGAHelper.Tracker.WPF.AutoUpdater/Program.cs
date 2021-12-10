using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MTGAHelper.Tracker.WPF.AutoUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the MTGAHelper Tracker Auto-updater. This will remove the current version and install the latest one.");
            Console.WriteLine("----------------------------------------");

            // Download the latest version
            var localFilepathMsi = Path.Combine(Path.GetTempPath(), "MTGAHelperTracker.msi");
            DownloadLatest(localFilepathMsi);

            // Install the downloaded file
            var fileAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper", "appsettings.json");
            var fileAppSettingsCopy = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper", "appsettings.json.bak");
            InstallLatest(fileAppSettings, fileAppSettingsCopy, localFilepathMsi);

            // Launch MTGAHelper
            RunProgram();
            Thread.Sleep(3000);
        }

        private static void DownloadLatest(string localFilepath)
        {
            Console.WriteLine("Downloading the latest version...");
            using (WebClient c = new WebClient())
            {
                c.DownloadFile("https://www.mtgahelper.com/download/MTGAHelperTracker.msi", localFilepath);
            }
            Console.WriteLine($"Download complete ({localFilepath})");
            Console.WriteLine("----------------------------------------");
        }

        private static void InstallLatest(string fileAppSettings, string fileAppSettingsCopy, string msiFileToRun)
        {
            var hadDesktopShortcut = File.Exists(GetDesktopShortcut());

            // Keep the appsettings.json
            var keepAppSettings = File.Exists(fileAppSettings);
            if (keepAppSettings)
                File.Copy(fileAppSettings, fileAppSettingsCopy, true);

            Console.WriteLine("Installing the latest version...");

            Process pInstall = new Process();
            var folderInstalled = GetFolderInstalled();

            if (folderInstalled == null)
            {
                Console.WriteLine("No previous version was found installed");
                pInstall.StartInfo.UseShellExecute = true;
                pInstall.StartInfo.FileName = msiFileToRun;
            }
            else
            {
                Console.WriteLine($"Upgrading the version found at '{folderInstalled}'");

                var localFilepathMsiOld = Path.Combine(Path.GetTempPath(), "MTGAHelperTracker1_8_5.msi");
                using (WebClient c = new WebClient())
                    c.DownloadFile("https://www.mtgahelper.com/download/MTGAHelperTracker1_8_5.msi", localFilepathMsiOld);
                var pUninstall = new Process();

                pUninstall.StartInfo.UseShellExecute = true;
                pUninstall.StartInfo.FileName = "msiexec";
                pUninstall.StartInfo.Arguments = $"/x \"{localFilepathMsiOld}\" /passive TARGETDIR=\"{folderInstalled}\"";
                pUninstall.Start();
                pUninstall.WaitForExit();

                //pInstall.StartInfo.FileName = "msiexec";
                //pInstall.StartInfo.Arguments = $"/i \"{msiFileToRun}\" /passive TARGETDIR=\"{folderInstalled}\"";
                pInstall.StartInfo.UseShellExecute = true;
                pInstall.StartInfo.FileName = msiFileToRun;
            }
            pInstall.Start();
            pInstall.WaitForExit();

            // Keep the appsettings.json settings
            if (keepAppSettings)
                KeepAppSettings(fileAppSettingsCopy, fileAppSettings);

            var shortcutDesktop = GetDesktopShortcut();
            if (hadDesktopShortcut == false && File.Exists(shortcutDesktop))
            {
                File.Delete(shortcutDesktop);
            }

            Console.WriteLine("Installation complete! Enjoy the latest version of MTGAHelper Tracker :)");
        }

        private static string GetFolderInstalled()
        {
            var folderInstalled = GetFolderInstalledInner("MTGAHelper.Tracker.WPF.exe");
            if (folderInstalled == null)
            {
                folderInstalled = GetFolderInstalledInner("MTGAHelper.Tracker.WPF.dll");
            }
            return folderInstalled;
        }

        private static string GetFolderInstalledInner(string file)
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

            var filePath = dllKey.Name.Replace($@"HKEY_CURRENT_USER\{keyFolder}\", "").Replace("|", @"\");
            var installationFolder = Path.GetDirectoryName(filePath);
            return installationFolder;
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

        private static void RunProgram()
        {
            string pathAppLink = Path.Combine(GetFolderInstalled(), "MTGAHelper.Tracker.WPF.exe");

            if (File.Exists(pathAppLink))
            {
                Console.WriteLine("Launching the MTGAHelper tracker...Good bye!");
                Process pTracker = new Process();
                pTracker.StartInfo.FileName = pathAppLink;
                pTracker.StartInfo.UseShellExecute = true;
                pTracker.Start();
            }
        }

        private static string GetDesktopShortcut()
        {
            var pathAppLink = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTGAHelper Tracker.lnk");
            if (File.Exists(pathAppLink) == false)
            {
                pathAppLink = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTGAHelper.lnk");
            }

            return pathAppLink;
        }
    }
}