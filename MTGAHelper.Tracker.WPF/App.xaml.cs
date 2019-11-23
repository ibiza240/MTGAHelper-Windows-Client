using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MtgaHelper.Web.Models.IoC;
using MTGAHelper.Entity;
using MTGAHelper.Entity.IoC;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.EventsSchedule;
using MTGAHelper.Lib.IO.Reader;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.Ioc;
using MTGAHelper.Lib.OutputLogParser.IoC;
//using MTGAHelper.Lib.Ioc;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Exceptions;
using MTGAHelper.Tracker.WPF.IoC;
using MTGAHelper.Tracker.WPF.Logging;
using MTGAHelper.Tracker.WPF.Views;
using MTGAHelper.Web.Models;
using MTGAHelper.Web.Models.Response.Misc;
using MTGAHelper.Web.Models.Response.SharedDto;
using MTGAHelper.Web.UI.IoC;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF
{
    static class Program
    {
        static Mutex mutex;
        [STAThread]
        static void Main()
        {
            // https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
            string mutexId = "Global\\{24eb5c47-62c6-4822-b305-b6265a3fbea3}";
            using (mutex = new Mutex(false, mutexId, out bool createdNew))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(1000, false);
                        if (hasHandle == false)
                            throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        hasHandle = true;
                    }

                    var application = new App();
                    application.InitializeComponent();
                    application.Run();
                }
                catch (TimeoutException)
                {
                    MessageBox.Show("Cannot run MTGAHelper more than once", "MTGAHelper");
                }
                finally
                {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }

    public partial class App : Application
    {
#if DEBUG
        const string server = "https://localhost:5001";
#else
        const string server = "https://mtgahelper.com";
#endif

        MainWindow mainWindow;
        ConfigModelApp configApp;
        IServiceProvider provider;
        HttpClientFactory httpClientFactory = new HttpClientFactory();

        string folderForConfigAndLog;
        string filePathAllCards;
        string filePathDateFormats;

        CacheSingleton<Dictionary<int, Card>> cacheCards;

        protected override void OnStartup(StartupEventArgs e)
        {
            StartApp();
        }

        private void StartApp()
        {
            try
            {
#if DEBUG || DEBUGWITHSERVER
                folderForConfigAndLog = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#else
             folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
#endif
                ConfigureApp();

                CheckForServerMessage();

                SetDateFormats();
                cacheCards = SetAllCards();

                // This requires cacheCards to be ready
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile(provider.GetService<MapperProfileEntity>());
                    cfg.AddProfile(new MapperProfileWebModels(cacheCards.Get(), provider, provider.GetService<UtilManaCurve>()));
                    cfg.AddProfile<MapperProfileTrackerWpf>();
                    cfg.AddProfile<MapperProfileLibOutputLogParser>();
                    cfg.AddProfile(new MapperProfileLibCardConvert(
                        provider.GetService<DeckListConverter>(),
                        provider.GetService<SingletonEventsScheduleManager>()));
                });

                mainWindow = provider.GetRequiredService<MainWindow>();
                LoadMainWindow();
            }
            catch (ServerNotAvailableException)
            {
                //MessageBox.Show("The server is not available at this moment, please retry in a few minutes. If you want to report this downtime, please leave a message on the Discord server (https://discord.gg/GTd3RMd)", "MTGAHelper");
                Shutdown();
            }
            catch (Exception ex)
            {
                //var userId = "N/A";
                //var fileAppSettings = Path.Combine(folderForConfigAndLog, "appSettings.json");
                //if (File.Exists(fileAppSettings))
                {
                    try
                    {
                        //string fileContent = "";
                        //using (FileStream fs = new FileStream(fileAppSettings, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        //using (var sr = new StreamReader(fs))
                        //    fileContent = sr.ReadToEnd();

                        //var appSettings = TryDeserializeJson<ConfigModelApp>(fileContent, true);
                        //userId = mainWindow.vm.Account.MtgaHelperUserId; //appSettings.UserId;
                        Log.Error(ex, "Error at startup:");
                        MessageBox.Show($"Error at startup: {ex.Message}{Environment.NewLine}{Environment.NewLine}You can check the latest application log file for details or contact me on Discord if you need more help.", "MTGAHelper");
                    }
                    catch
                    {
                        // Ignore errors
                        MessageBox.Show("Fatal error. Please contact me on the Discord server so we can troubleshoot that");
                    }
                }

                //new Business.ServerApiCaller(null, null).LogErrorRemote(userId, Web.UI.Model.Request.ErrorTypeEnum.Startup, ex);
                Shutdown();
            }
        }

        void CheckForServerMessage()
        {
            var msgs = TryDownloadFromServer(() =>
                {
                    var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/TrackerClientMessages", 15).Result;
                    return TryDeserializeJson<Dictionary<string, string>>(raw, true);
                }
            );

            if (msgs != null)
            {
                var version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
                if (msgs.ContainsKey(version))
                {
                    MessageBox.Show(msgs[version], "Message from server");
                }
            }
        }

        void ConfigureApp()
        {
            void RecreateAppSettings(string fileAppSettings)
            {
                var defaultSettings = JsonConvert.SerializeObject(new ConfigModelApp());
                File.WriteAllText(fileAppSettings, defaultSettings);
            }

            var fileAppSettings = Path.Combine(folderForConfigAndLog, "appsettings.json");
            string fileContent = "";
            if (File.Exists(fileAppSettings) == false || new FileInfo(fileAppSettings).Length == 0)
            {
                RecreateAppSettings(fileAppSettings);
            }
            try
            {
                 fileContent = File.ReadAllText(fileAppSettings);
                var appSettings = JsonConvert.DeserializeObject<ConfigModelApp>(fileContent);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"AppSettings file was invalid:{Environment.NewLine}{fileContent}", fileContent);
                RecreateAppSettings(fileAppSettings);
            }

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
                    Path.Combine(folderForConfigAndLog, "log-.txt"),
                    rollingInterval: RollingInterval.Month,
                    outputTemplate: "{Timestamp:dd HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Write(LogEventLevel.Information, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .RegisterServicesApp(configuration)
                .RegisterServicesLibOutputLogParser()
                .RegisterServicesWebModels()
                .RegisterServicesEntity();

            provider = serviceCollection.BuildServiceProvider();

            var configAppLib = provider.GetService<IOptionsMonitor<MTGAHelper.Lib.Config.ConfigModelApp>>();
            var folderData = Path.Combine(folderForConfigAndLog, "data");
            Directory.CreateDirectory(folderData);
            configAppLib.CurrentValue.FolderData = folderData;
            configApp = provider.GetService<IOptionsMonitor<ConfigModelApp>>().CurrentValue;
            filePathAllCards = Path.Combine(folderData, "AllCardsCached.json");
            filePathDateFormats = Path.Combine(folderForConfigAndLog, "data", "dateFormats.json");
        }

        void SetDateFormats()
        {
            var mustDownload = true;

            if (File.Exists(filePathDateFormats))
            {
                string fileContent = "";
                using (FileStream fs = new FileStream(filePathDateFormats, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                    fileContent = sr.ReadToEnd();

                if (TryDeserializeJson<List<string>>(fileContent) == null)
                    File.Delete(filePathDateFormats);
                else
                    mustDownload = false;
            }

            if (mustDownload)
            {
                var dateFormatsResponse = TryDownloadFromServer(() =>
                {
                    var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/dateFormats", 15).Result;
                    var response = TryDeserializeJson<GetDateFormatsResponse>(raw);
                    return response;
                });

                if (dateFormatsResponse != null)
                    File.WriteAllText(filePathDateFormats, JsonConvert.SerializeObject(dateFormatsResponse.DateFormats));
            }
        }

        CacheSingleton<Dictionary<int, Card>> SetAllCards()
        {
            var cacheCards = provider.GetService<CacheSingleton<Dictionary<int, Card>>>();

            // First try to get all cards from the local file
            ICollection<Card> allCards = GetAllCardsFromDisk();
            if (allCards == null)
            {
                // We need to download all cards remotely
                var cardsResponse = TryDownloadFromServer(DownloadAllCards);
                if (cardsResponse != null)
                {
                    // Cannot use AutoMapper since at this point it has not been initialized yet
                    //allCards = Mapper.Map<ICollection<Card>>(cardsResponse.Cards);
                    allCards = JsonConvert.DeserializeObject<ICollection<Card>>(JsonConvert.SerializeObject(cardsResponse.Cards));
                    SaveAllCardsToDisk(allCards);
                }
            }

            cacheCards.PopulateIfNotSet(() => allCards.ToDictionary(i => i.grpId, i => i));

            return cacheCards;
        }

        ICollection<Card> GetAllCardsFromDisk()
        {
            if (File.Exists(filePathAllCards))
            {
                string fileContent = "";
                using (FileStream fs = new FileStream(filePathAllCards, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                    fileContent = sr.ReadToEnd();

                var hashAllCardsLocal = new Entity.Util().To32BitFnv1aHash(fileContent);
                var hashAllCardsRemote = TryDownloadFromServer(DownloadHashAllCards);

                if (hashAllCardsLocal == hashAllCardsRemote)
                {
                    // Local file for cards is up-to-date
                    // Will return null if the JSON is invalid and cards will be redownloaded from the server
                    return TryDeserializeJson<List<Card>>(fileContent);
                }
                else
                {
                    Log.Information("Local cards out of date ({localHash}), must redownload ({remoteHash})", hashAllCardsLocal, hashAllCardsRemote);
                    File.Delete(filePathAllCards);
                }
            }

            return null;
        }

        uint DownloadHashAllCards()
        {
            var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/lastcardshash", 15).Result;
            var response = TryDeserializeJson<LastHashResponse>(raw);
            return Convert.ToUInt32(response.LastHash);
        }

        GetCardsResponse DownloadAllCards()
        {
            var raw = HttpClientGet_WithTimeoutNotification(server + "/api/misc/cards", 60).Result;
            var response = TryDeserializeJson<GetCardsResponse>(raw);
            return response;
        }

        public async Task<string> HttpClientGet_WithTimeoutNotification(string requestUri, double timeout)
        {
            using (HttpClient client = httpClientFactory.Create(timeout))
            {
                var doRequest = true;
                while (doRequest)
                {
                    try
                    {
                        var raw = await client.GetAsync(requestUri).Result.Content.ReadAsStringAsync();
                        return raw;
                    }
                    catch (Exception ex)
                    {
                        doRequest = MessageBox.Show($"It appears the server didn't reply in time ({timeout} seconds). Do you want to retry? Choosing No will stop the program.{Environment.NewLine}{Environment.NewLine}Maybe the server is down? Go check https://mtgahelper.com and if that is the case, please retry later.",
                            "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                        if (doRequest == false)
                            throw new ServerNotAvailableException();
                    }
                }

                return "";
            }
        }

        void SaveAllCardsToDisk(ICollection<Card> allCards)
        {
            // Loop is a patch to try to stop having the error cannot access the file ... because it is being used by another process.
            int iTry = 0;
            while (iTry < 5)
            {
                try
                {
                    iTry++;
                    File.WriteAllText(filePathAllCards, JsonConvert.SerializeObject(allCards));
                    break;
                }
                catch (IOException ex)
                {
                    if (iTry >= 5)
                        throw new ApplicationException("Could not write AllCards.json:", ex);
                    else
                        Thread.Sleep(1000);
                }
            }
        }

        void LoadMainWindow()
        {
            mainWindow.SetAlwaysOnTop(configApp.AlwaysOnTop);

            if ((configApp.WindowSettings?.Position?.X ?? 0) != 0 ||
                (configApp.WindowSettings?.Position?.Y ?? 0) != 0 ||
                (configApp.WindowSettings?.Size?.X ?? 0) != 0 ||
                (configApp.WindowSettings?.Size?.Y ?? 0) != 0)
            {
                mainWindow.Width = (double)configApp.WindowSettings?.Size?.X;
                mainWindow.Height = (double)configApp.WindowSettings?.Size?.Y;

                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Left = (double)configApp.WindowSettings?.Position?.X;
                mainWindow.Top = (double)configApp.WindowSettings?.Position?.Y;
            }

            mainWindow.Show();
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mainWindow != null && mainWindow.configApp != null)
            {
                mainWindow.configApp.AlwaysOnTop = mainWindow.Topmost;
                mainWindow.configApp.Opacity = mainWindow.vm.Opacity;
                mainWindow.configApp.WindowSettings = new WindowSettings
                {
                    Position = new Config.Point((int)mainWindow.Left, (int)mainWindow.Top),
                    Size = new Config.Point((int)mainWindow.Width, (int)mainWindow.Height),
                };

                mainWindow.configApp.Save();
            }
        }

        T TryDownloadFromServer<T>(Func<T> action)
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
                if (subEx is HttpRequestException)
                    throw new ServerNotAvailableException(ex);
                else if (subEx is ServerNotAvailableException)
                    throw subEx;
                throw;
            }
        }

        T TryDeserializeJson<T>(string json, bool defaultNew = false) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException)
            {
                return defaultNew ? new T() : default(T);
            }
        }
    }
}
