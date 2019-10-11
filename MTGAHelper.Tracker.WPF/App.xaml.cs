using AutoMapper;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF
{
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

        string folderForConfigAndLog;
        string filePathAllCards;
        string filePathDateFormats;

        CacheSingleton<ICollection<Card>> cacheCards;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
#if DEBUG || DEBUGWITHSERVER
                folderForConfigAndLog = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#else
             folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
#endif

                CheckForServerMessage();

                ConfigureApp();

                SetDateFormats();
                cacheCards = SetAllCards();

                // This requires cacheCards to be ready
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile(new MapperProfileEntity(cacheCards));
                    cfg.AddProfile(new MapperProfileWebModels(cacheCards.Get(), provider, provider.GetService<UtilManaCurve>()));
                    cfg.AddProfile<MapperProfileTrackerWpf>();
                    cfg.AddProfile<MapperProfileLibOutputLogParser>();
                    cfg.AddProfile(new MapperProfileLibCardConvert(
                        cacheCards,
                        provider.GetService<DeckListConverter>(),
                        provider.GetService<SingletonEventsScheduleManager>()));
                });

                mainWindow = provider.GetRequiredService<MainWindow>();
                LoadMainWindow();
            }
            catch (ServerNotAvailableException)
            {
                MessageBox.Show("The server is not available at this moment, please retry in a few minutes. If you want to report this downtime, please leave a message on the Discord server (https://discord.gg/GTd3RMd)");
                Shutdown();
            }
            catch (Exception ex)
            {
                var userId = "N/A";
                var fileAppSettings = Path.Combine(folderForConfigAndLog, "appSettings.json");
                if (File.Exists(fileAppSettings))
                {
                    try
                    {
                        string fileContent = "";
                        using (FileStream fs = new FileStream(fileAppSettings, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs))
                            fileContent = sr.ReadToEnd();

                        var appSettings = TryDeserializeJson<ConfigModelApp>(fileContent, true);
                        userId = appSettings.UserId;
                        Log.Error(ex, "Error at startup:");
                    }
                    catch
                    {
                        // Ignore errors
                    }
                }

                new Business.ServerApiCaller(null, null).LogErrorRemote(userId, Web.UI.Model.Request.ErrorTypeEnum.Startup, ex);
                Shutdown();
            }
        }

        void CheckForServerMessage()
        {
            var msgs = TryDownloadFromServer(DownloadTrackerClientMessages);
            var version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            if (msgs.ContainsKey(version))
            {
                MessageBox.Show(msgs[version], "Message from server");
            }
        }

        Dictionary<string, string> DownloadTrackerClientMessages()
        {
            using (HttpClient client = new HttpClient())
            {
                var msgsRaw = client.GetAsync(server + "/api/misc/TrackerClientMessages").Result.Content.ReadAsStringAsync().Result;
                return TryDeserializeJson<Dictionary<string, string>>(msgsRaw, true);
            }
        }

        void ConfigureApp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(folderForConfigAndLog, "appsettings.json"), optional: false, reloadOnChange: true)
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
            configAppLib.CurrentValue.FolderData = folderData;
            configApp = provider.GetService<IOptionsMonitor<ConfigModelApp>>().CurrentValue;
            Directory.CreateDirectory(folderData);
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
                var dateFormatsResponse = TryDownloadFromServer(DownloadDateFormats);
                if (dateFormatsResponse != null)
                    File.WriteAllText(filePathDateFormats, JsonConvert.SerializeObject(dateFormatsResponse.DateFormats));
            }
        }

        GetDateFormatsResponse DownloadDateFormats()
        {
            using (HttpClient client = new HttpClient())
            {
                var dateFormats = client.GetAsync(server + "/api/misc/dateFormats").Result.Content.ReadAsStringAsync().Result;
                return TryDeserializeJson<GetDateFormatsResponse>(dateFormats);
            }
        }

        CacheSingleton<ICollection<Card>> SetAllCards()
        {
            var cacheCards = provider.GetService<CacheSingleton<ICollection<Card>>>();

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

            cacheCards.PopulateIfNotSet(() => allCards);

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
                    Log.Information("Local cards out of date, must redownload");
                    File.Delete(filePathAllCards);
                }
            }

            return null;
        }

        uint DownloadHashAllCards()
        {
            using (HttpClient client = new HttpClient())
            {
                var lastCardsHashRemoteRaw = client.GetAsync(server + "/api/misc/lastcardshash").Result.Content.ReadAsStringAsync().Result;
                return Convert.ToUInt32(TryDeserializeJson<LastHashResponse>(lastCardsHashRemoteRaw, true).LastHash);
            }
        }

        GetCardsResponse DownloadAllCards()
        {
            using (HttpClient client = new HttpClient())
            {
                var cardsRaw = client.GetAsync(server + "/api/misc/cards").Result.Content.ReadAsStringAsync().Result;
                var cardsResponse = TryDeserializeJson<GetCardsResponse>(cardsRaw);
                return cardsResponse;
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
                if (ex.InnerExceptions.First() is HttpRequestException)
                    throw new ServerNotAvailableException(ex);

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
