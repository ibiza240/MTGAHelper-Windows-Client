using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using MtgaHelper.Web.Models.IoC;
using MTGAHelper.Entity;
using MTGAHelper.Lib.IoC;
using MTGAHelper.Lib.OutputLogParser.IoC;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Exceptions;
using MTGAHelper.Tracker.WPF.IoC;
using MTGAHelper.Tracker.WPF.Logging;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Tracker.WPF.Views;
using MTGAHelper.Web.Models.Response.Misc;
using MTGAHelper.Web.UI.Model.Response.Misc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using SimpleInjector;

namespace MTGAHelper.Tracker.WPF
{
    public partial class App
    {
        private readonly StringBuilder StartupLogger = new StringBuilder();

        private MainWindow TrackerMainWindow;

        private ConfigModel ConfigApp;

        private Container Container;

        private readonly HttpClientFactory HttpClientFactory = new HttpClientFactory();

        private string FolderForConfigAndLog;

        private string FolderData;

        private DataDownloader dataDownloader = new DataDownloader();

        //CacheSingleton<Dictionary<int, Card>> cacheCards;

        protected override void OnStartup(StartupEventArgs e)
        {
            _ = StartApp();
        }

        private async Task StartApp()
        {
            try
            {
                // IMPORTANT! https://stackoverflow.com/a/33091871/215553
                // Web server is TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                StartupLogger.AppendLine("StartApp()");

                FolderForConfigAndLog = new DebugOrRelease().GetConfigFolder();

                FolderData = Path.Combine(FolderForConfigAndLog, "data");

                ConfigureApp();

                await CheckForServerMessage();

                await CheckForUpdate();

                await SetDateFormats();

                // Get the local file up-to-date
                StartupLogger.AppendLine("CheckDataAndDownloadIfOutOfDate(AllCardsCached2)");
                await CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum.AllCardsCached2);
                StartupLogger.AppendLine("CheckDataAndDownloadIfOutOfDate(draftRatings)");
                await CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum.draftRatings);
                StartupLogger.AppendLine("CheckDataAndDownloadIfOutOfDate(ConfigResolution)");
                await CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum.ConfigResolutions);

                // Set cache content from local file
                StartupLogger.AppendLine("Verifying container configuration");
                Container.Verify();

                // This requires cacheCards to be ready
                StartupLogger.AppendLine("cacheCards");

                // Add a log entry
                StartupLogger.AppendLine("LoadMainWindow()");

                // Create the main window instance
                TrackerMainWindow = new MainWindow(Container.GetInstance<MainWindowVM>());
            }
            catch (ServerNotAvailableException)
            {
                Shutdown();
            }
            catch (Exception ex)
            {
                try
                {
                    Log.Error(ex, "Error at startup:");
                    MessageBox.Show(
                        $"Error at startup: Check the latest application log file for details or contact me on Discord if you need more help.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                        "MTGAHelper");
                }
                catch
                {
                    // Ignore errors
                    MessageBox.Show("Fatal error. Please contact me on the Discord server so we can troubleshoot that");
                }

                Shutdown();
            }
            finally
            {
                File.WriteAllText(Path.Combine(FolderForConfigAndLog, "log-startup.txt"), StartupLogger.ToString());
            }
        }

        private async Task CheckForUpdate()
        {
            StartupLogger.AppendLine("CheckForUpdate()");
            string latestVersion = await TryDownloadFromServer(async () =>
            {
                string raw = await dataDownloader.HttpClientGet_WithTimeoutNotification(DebugOrRelease.Server + "/api/misc/VersionTracker", 15);
                return TryDeserializeJson<GetVersionTrackerResponse>(raw).Version;
            });

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            if (new Version(fvi.FileVersion) < new Version(latestVersion))
                MustDownloadNewVersion();

        }

        private static void MustDownloadNewVersion()
        {
            if (new DebugOrRelease().IsDebug)
                return;

            if (MessageBox.Show("A new version of the MTGAHelper Tracker is available, you must install it to continue. Proceed now?", "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
                string fileExe = Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.exe");

                var isDownloaded = false;
                int iRetry = 0;
                while (isDownloaded == false)
                {
                    try
                    {
                        // Download latest auto-updater
                        var wc = new WebClient();
                        wc.DownloadFile("https://mtgahelper.com/download/Newtonsoft.Json.dll", Path.Combine(folderForConfigAndLog, "Newtonsoft.Json.dll"));
                        wc.DownloadFile("https://mtgahelper.com/download/MTGAHelper.Tracker.AutoUpdater.exe", fileExe);
                        isDownloaded = true;
                    }
                    catch (Exception)
                    {
                        iRetry++;
                        // Sometimes connection closed unexpectedly
                        if (iRetry >= 5)
                        {
                            MessageBox.Show("Failed to download the files for updating. Maybe the server is down? Go check https://mtgahelper.com and if that is the case, please retry later.");
                            throw;
                        }
                    }
                }
                var ps = new ProcessStartInfo(fileExe)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(ps);
            }

            Current.Shutdown();
        }

        private async Task CheckForServerMessage()
        {
            StartupLogger.AppendLine("CheckForServerMessage()");

            var msgs = await TryDownloadFromServer(async () =>
                {
                    string raw = await dataDownloader.HttpClientGet_WithTimeoutNotification(DebugOrRelease.Server + "/api/misc/TrackerClientMessages", 15);
                    return TryDeserializeJson<Dictionary<string, string>>(raw, true);
                }
            );

            if (msgs == null) return;

            string version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

            if (msgs.ContainsKey(version))
            {
                MessageBox.Show(msgs[version], "Message from server");
            }
        }

        private void ConfigureApp()
        {
            StartupLogger.AppendLine("ConfigureApp()");

            void RecreateAppSettings(string appSettings)
            {
                StartupLogger.AppendLine("RecreateAppSettings()");
                string defaultSettings = JsonConvert.SerializeObject(new ConfigModel());
                File.WriteAllText(appSettings, defaultSettings);
            }

            string fileAppSettings = Path.Combine(FolderForConfigAndLog, "appsettings.json");

            if (File.Exists(fileAppSettings) == false || new FileInfo(fileAppSettings).Length == 0)
            {
                RecreateAppSettings(fileAppSettings);
            }

            var fileContent = "";

            try
            {
                fileContent = File.ReadAllText(fileAppSettings).Trim().Replace("\0", string.Empty);
                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    RecreateAppSettings(fileAppSettings);
                    fileContent = File.ReadAllText(fileAppSettings);
                }

                JsonConvert.DeserializeObject<ConfigModel>(fileContent);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"AppSettings file was invalid:{Environment.NewLine}{fileContent}", fileContent);
                RecreateAppSettings(fileAppSettings);
            }

            StartupLogger.AppendLine("Create logger");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(fileAppSettings, optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
#if DEBUG || DEBUGWITHSERVER
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                //.Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("outputLogError")))
                .Enrich.With(new RemovePropertiesEnricher())
                .WriteTo.File(
                    Path.Combine(FolderForConfigAndLog, "log-.txt"),
                    rollingInterval: RollingInterval.Month,
                    outputTemplate: "{Timestamp:dd HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Write(LogEventLevel.Information, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            StartupLogger.AppendLine("Build services");

            ConfigApp = configuration.Get<ConfigModel>();

            ConfigApp.LogFilePath = ConfigApp.LogFilePath.Replace("output_log.txt", "Player.log");

            Container = CreateContainer(ConfigApp, FolderData);

            Directory.CreateDirectory(FolderData);
        }

        /// <summary>
        /// Create the DI container
        /// ReSharper disable once MemberCanBePrivate.Global
        /// </summary>
        /// <param name="configModelApp"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        public static Container CreateContainer(ConfigModel configModelApp, string folderData)
        {
            var container = new Container();
            //container.Options.ConstructorResolutionBehavior = new InternalConstructorResolutionBehavior(container);

            //container.Register<IInputOutputOrchestrator, InputOutputOrchestratorMock>();

            return container
                .RegisterDataPath(folderData)
                .RegisterServicesApp(configModelApp)
                .RegisterServicesLibOutputLogParser()
                .RegisterFileLoaders()
                //.RegisterServicesDrafHelperShared()
                //.RegisterServicesDrafHelper(new ConfigDraftHelper
                //{
                //    FolderData = folderData,
                //})
                .RegisterServicesTracker()
                .RegisterServicesWebModels()
                .RegisterServicesShared()
                .RegisterMapperConfig();
        }

        private async Task SetDateFormats()
        {
            StartupLogger.AppendLine("SetDateFormats()");

            var mustDownload = true;

            string filePathDateFormats = Path.Combine(FolderData, "dateFormats.json");

            if (File.Exists(filePathDateFormats))
            {
                string fileContent;
                using (var fs = new FileStream(filePathDateFormats, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var sr = new StreamReader(fs);
                    fileContent = sr.ReadToEnd();
                }

                if (TryDeserializeJson<List<string>>(fileContent) == null)
                    File.Delete(filePathDateFormats);
                else
                    mustDownload = false;
            }

            if (!mustDownload) return;

            var dateFormatsResponse = await TryDownloadFromServer(async () =>
            {
                string raw = await dataDownloader.HttpClientGet_WithTimeoutNotification(DebugOrRelease.Server + "/api/misc/dateFormats", 15);
                var response = TryDeserializeJson<GetDateFormatsResponse>(raw);
                return response;
            });

            if (dateFormatsResponse != null)
                File.WriteAllText(filePathDateFormats, JsonConvert.SerializeObject(dateFormatsResponse.DateFormats));
        }

        private async Task CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum dataFileType)
        {
            // Load file content and calculate hash
            var fileContent = "";

            string filePath = Path.Combine(FolderData, $"{dataFileType}.json");

            if (File.Exists(filePath))
                fileContent = File.ReadAllText(filePath);

            uint hashLocal = Fnv1aHasher.To32BitFnv1aHash(fileContent);

            // Compare hash to server
            bool isUpToDate = await TryDownloadFromServer(async () =>
            {
                StartupLogger.AppendLine($"isUpToDate({dataFileType})");
                string raw = await dataDownloader.HttpClientGet_WithTimeoutNotification(DebugOrRelease.Server + $"/api/misc/filehash?id={dataFileType}&hash={hashLocal}", 15);
                var response = TryDeserializeJson<bool>(raw);
                return response;
            });

            // Download if out of date
            if (isUpToDate == false)
            {
                StartupLogger.AppendLine($"redownloading({dataFileType})");
                Log.Information("Data [{dataType}] out of date, redownloading", dataFileType);
                await TryDownloadFromServer(async () =>
                {
                    await dataDownloader.HttpClientDownloadFile_WithTimeoutNotification(DebugOrRelease.Server + $"/api/download/{dataFileType}", 60, filePath);
                });
            }
        }

        //void SaveAllCardsToDisk(ICollection<Card> allCards)
        //{
        //    // Loop is a patch to try to stop having the error cannot access the file ... because it is being used by another process.
        //    int iTry = 0;
        //    while (iTry < 5)
        //    {
        //        try
        //        {
        //            iTry++;
        //            File.WriteAllText(filePathAllCards, JsonConvert.SerializeObject(allCards));
        //            break;
        //        }
        //        catch (IOException ex)
        //        {
        //            if (iTry >= 5)
        //                throw new ApplicationException("Could not write AllCards.json:", ex);
        //            else
        //                Thread.Sleep(1000);
        //        }
        //    }
        //}

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TrackerMainWindow?.ViewModel?.Config?.Save();
        }

        private static T TryDownloadFromServer<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (HttpRequestException ex)
            {
                throw new ServerNotAvailableException(ex);
            }
            catch (AggregateException ex)
            {
                var subEx = ex.InnerExceptions.First();
                switch (subEx)
                {
                    case HttpRequestException _:
                        throw new ServerNotAvailableException(ex);
                    case ServerNotAvailableException _:
                        throw subEx;
                    default:
                        throw;
                }
            }
        }

        private static T TryDeserializeJson<T>(string json, bool defaultNew = false) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException)
            {
                return defaultNew ? new T() : default;
            }
        }
    }
}
