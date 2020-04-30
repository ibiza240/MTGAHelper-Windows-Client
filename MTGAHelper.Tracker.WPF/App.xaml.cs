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
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using MtgaHelper.Web.Models.IoC;
using MTGAHelper.Entity;
using MTGAHelper.Entity.IoC;
using MTGAHelper.Lib.OutputLogParser.IoC;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Exceptions;
using MTGAHelper.Tracker.WPF.IoC;
using MTGAHelper.Tracker.WPF.Logging;
using MTGAHelper.Tracker.WPF.Tools;
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
        private const string SERVER = DebugOrRelease.Server;

        private readonly StringBuilder StartupLogger = new StringBuilder();

        private MainWindow TrackerMainWindow;

        private ConfigModel ConfigApp;

        private Container Container;

        private readonly HttpClientFactory HttpClientFactory = new HttpClientFactory();

        private string FolderForConfigAndLog;

        private string FolderData;

        //CacheSingleton<Dictionary<int, Card>> cacheCards;

        protected override void OnStartup(StartupEventArgs e)
        {
            StartApp();
        }

        private async Task StartApp()
        {
            try
            {
                StartupLogger.AppendLine("StartApp()");

                FolderForConfigAndLog = new DebugOrRelease().GetConfigFolder();

                FolderData = Path.Combine(FolderForConfigAndLog, "data");

                ConfigureApp();

                CheckForServerMessage();

                CheckForUpdate();

                SetDateFormats();

                // Get the local file up-to-date
                StartupLogger.AppendLine("CheckDataAndDownloadIfOutOfDate(AllCardsCached2)");
                await CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum.AllCardsCached2);
                StartupLogger.AppendLine("CheckDataAndDownloadIfOutOfDate(draftRatings)");
                await CheckDataAndDownloadIfOutOfDate(DataFileTypeEnum.draftRatings);

                // Set cache content from local file
                StartupLogger.AppendLine("Verifying container configuration");
                //Container.Verify();

                // This requires cacheCards to be ready
                StartupLogger.AppendLine("cacheCards");


                Mapper.Initialize(Container.GetInstance<MapperConfigurationExpression>());
                Mapper.AssertConfigurationIsValid();

                // Add a log entry
                StartupLogger.AppendLine("LoadMainWindow()");

                // Create the main window instance
                TrackerMainWindow = Container.GetInstance<MainWindow>();
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

        private void CheckForUpdate()
        {
            StartupLogger.AppendLine("CheckForUpdate()");
            string latestVersion = TryDownloadFromServer(() =>
            {
                string raw = HttpClientGet_WithTimeoutNotification(SERVER + "/api/misc/VersionTracker", 15).Result;
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
                // Download latest auto-updater
                string folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
                string fileExe = Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.exe");
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/Newtonsoft.Json.dll", Path.Combine(folderForConfigAndLog, "Newtonsoft.Json.dll"));
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.dll", Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.dll"));
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.exe", fileExe);
                new WebClient().DownloadFile("https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.Tracker.AutoUpdater.runtimeconfig.json", Path.Combine(folderForConfigAndLog, "MTGAHelper.Tracker.AutoUpdater.runtimeconfig.json"));

                var ps = new ProcessStartInfo(fileExe)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(ps);
            }

            Current.Shutdown();
        }

        private void CheckForServerMessage()
        {
            StartupLogger.AppendLine("CheckForServerMessage()");

            var msgs = TryDownloadFromServer(() =>
                {
                    string raw = HttpClientGet_WithTimeoutNotification(SERVER + "/api/misc/TrackerClientMessages", 15).Result;
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

            Container = CreateContainer(ConfigApp, FolderData);

            Directory.CreateDirectory(FolderData);
        }

        public static Container CreateContainer(ConfigModel configModelApp, string folderData)
        {
            return new Container()
                .RegisterDataPath(folderData)
                .RegisterServicesApp(configModelApp)
                .RegisterServicesLibOutputLogParser()
                .RegisterFileLoaders()
                .RegisterServicesTracker()
                .RegisterServicesWebModels()
                .RegisterServicesEntity()
                .RegisterMapperConfig();
        }

        private void SetDateFormats()
        {
            StartupLogger.AppendLine("SetDateFormats()");

            var mustDownload = true;

            string filePathDateFormats = Path.Combine(FolderForConfigAndLog, "data", "dateFormats.json");

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

            var dateFormatsResponse = TryDownloadFromServer(() =>
            {
                string raw = HttpClientGet_WithTimeoutNotification(SERVER + "/api/misc/dateFormats", 15).Result;
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

            uint hashLocal = new Util().To32BitFnv1aHash(fileContent);

            // Compare hash to server
            bool isUpToDate = TryDownloadFromServer(() =>
            {
                StartupLogger.AppendLine($"isUpToDate({dataFileType})");
                string raw = HttpClientGet_WithTimeoutNotification(SERVER + $"/api/misc/filehash?id={dataFileType}&hash={hashLocal}", 15).Result;
                var response = TryDeserializeJson<bool>(raw);
                return response;
            });

            // Download if out of date
            if (isUpToDate == false)
            {
                StartupLogger.AppendLine($"redownloading({dataFileType})");
                Log.Information("Data [{dataType}] out of date, redownloading", dataFileType);
                await TryDownloadFromServer<Task<object>>(async () =>
                {
                    await HttpClientDownloadFile_WithTimeoutNotification(SERVER + $"/api/download/{dataFileType}", 60, filePath);
                    return null;
                });
            }
        }

        //void LoadCacheAllCards()
        //{
        //    // First try to get all cards from the local file
        //    ICollection<Card> allCards = GetAllCardsFromDisk();
        //    if (allCards == null)
        //    {
        //        // We need to download all cards remotely
        //        var cardsResponse = TryDownloadFromServer(() =>
        //        {
        //            var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/cards", 60).Result;
        //            var response = TryDeserializeJson<GetCardsResponse>(raw);
        //            return response;
        //        });

        //        if (cardsResponse != null)
        //        {
        //            // Cannot use AutoMapper since at this point it has not been initialized yet
        //            //allCards = Mapper.Map<ICollection<Card>>(cardsResponse.Cards);
        //            allCards = JsonConvert.DeserializeObject<ICollection<Card>>(JsonConvert.SerializeObject(cardsResponse.Cards));
        //            SaveAllCardsToDisk(allCards);
        //        }
        //    }

        //    cacheCards = provider.GetService<CacheSingleton<Dictionary<int, Card>>>();
        //    cacheCards.PopulateIfNotSet(() => allCards.ToDictionary(i => i.grpId, i => i));
        //}

        //ICollection<Card> GetAllCardsFromDisk()
        //{
        //    if (File.Exists(filePathAllCards))
        //    {
        //        string fileContent = "";
        //        using (FileStream fs = new FileStream(filePathAllCards, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        using (var sr = new StreamReader(fs))
        //            fileContent = sr.ReadToEnd();

        //        var hashAllCardsLocal = new Entity.Util().To32BitFnv1aHash(fileContent);
        //        var hashAllCardsRemote = TryDownloadFromServer(() =>
        //        {
        //            var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/lastcardshash", 15).Result;
        //            var response = TryDeserializeJson<LastHashResponse>(raw);
        //            return Convert.ToUInt32(response.LastHash);
        //        });

        //        if (hashAllCardsLocal == hashAllCardsRemote)
        //        {
        //            // Local file for cards is up-to-date
        //            // Will return null if the JSON is invalid and cards will be redownloaded from the server
        //            return TryDeserializeJson<List<Card>>(fileContent);
        //        }
        //        else
        //        {
        //            Log.Information("Local cards out of date ({localHash}), must redownload ({remoteHash})", hashAllCardsLocal, hashAllCardsRemote);
        //            File.Delete(filePathAllCards);
        //        }
        //    }

        //    return null;
        //}

        private async Task HttpClientDownloadFile_WithTimeoutNotification(string requestUri, double timeout, string filePath)
        {
            using HttpClient client = HttpClientFactory.Create(timeout);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            using Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);

            await contentStream.CopyToAsync(stream);
        }

        private async Task<string> HttpClientGet_WithTimeoutNotification(string requestUri, double timeout)
        {
            using var client = HttpClientFactory.Create(timeout);

            while (true)
            {
                try
                {
                    string raw = await client.GetAsync(requestUri).Result.Content.ReadAsStringAsync();
                    return raw;
                }
                catch (Exception)
                {
                    bool doRequest = MessageBox.Show($"It appears the server didn't reply in time ({timeout} seconds). Do you want to retry? Choosing No will stop the program.{Environment.NewLine}{Environment.NewLine}Maybe the server is down? Go check https://mtgahelper.com and if that is the case, please retry later.",
                        "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                    if (doRequest == false)
                        throw new ServerNotAvailableException();
                }
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
            TrackerMainWindow?.ConfigModel?.Save();
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
