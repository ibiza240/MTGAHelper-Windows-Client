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
using MTGAHelper.Tracker.WPF.IoC;
using MTGAHelper.Tracker.WPF.Logging;
using MTGAHelper.Tracker.WPF.Views;
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

        IConfiguration configuration;
        IServiceProvider provider;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Get broadcast message
            using (HttpClient client = new HttpClient())
            {
                var msgsRaw = client.GetAsync(server + "/api/misc/TrackerClientMessages").Result.Content.ReadAsStringAsync().Result;
                var msgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(msgsRaw);
                var version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
                if (msgs.ContainsKey(version))
                {
                    MessageBox.Show(msgs[version], "Message from server");
                }
            }

#if DEBUG || DEBUGWITHSERVER
            var folderForConfigAndLog = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#else
            var folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
#endif

            try
            {
                configuration = new ConfigurationBuilder()
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

                var configApp = provider.GetService<IOptionsMonitor<MTGAHelper.Lib.Config.ConfigModelApp>>();
                configApp.CurrentValue.FolderData = Path.Combine(folderForConfigAndLog, "data");

                var cacheCards = provider.GetService<CacheSingleton<ICollection<Card>>>();
                cacheCards.PopulateIfNotSet(() =>
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Directory.CreateDirectory(configApp.CurrentValue.FolderData);
                            var fileOnDisk = Path.Combine(configApp.CurrentValue.FolderData, "AllCardsCached.json");
                            if (File.Exists(fileOnDisk))
                            {
                                var fileContent = File.ReadAllText(fileOnDisk);
                                var lastCardsHashLocal = new Util().To32BitFnv1aHash(fileContent);
                                var lastCardsHashRemoteRaw = client.GetAsync(server + "/api/misc/lastcardshash").Result.Content.ReadAsStringAsync().Result;
                                var lastCardsHashRemote = Convert.ToUInt32(JsonConvert.DeserializeObject<LastHashResponse>(lastCardsHashRemoteRaw).LastHash);
                                if (lastCardsHashLocal == lastCardsHashRemote)
                                {
                                    // Local file for cards is up-to-date
                                    return JsonConvert.DeserializeObject<ICollection<Card>>(fileContent);
                                }
                            }

                            // Must download cards
                            var cardsRaw = client.GetAsync(server + "/api/misc/cards").Result.Content.ReadAsStringAsync().Result;
                            var cardsResponse = JsonConvert.DeserializeObject<GetCardsResponse>(cardsRaw);
                            var allCards = JsonConvert.DeserializeObject<ICollection<Card>>(JsonConvert.SerializeObject(cardsResponse.Cards));
                            File.WriteAllText(fileOnDisk, JsonConvert.SerializeObject(allCards));
                            return allCards;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("The MTGAHelper servers are unavailable at the moment, please try again in a few minutes. Sorry for the inconvenience.", "MTGAHelper");
                        App.Current.Shutdown();
                        return new Card[0];
                    }
                });

                var eventsScheduleManager = provider.GetService<SingletonEventsScheduleManager>();

                // Date formats
                var fileDateFormats = Path.Combine(folderForConfigAndLog, "data", "dateFormats.json");
                if (File.Exists(fileDateFormats) == false)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var dateFormats = client.GetAsync(server + "/api/misc/dateFormats").Result.Content.ReadAsStringAsync().Result;
                        var dateFormatsResponse = JsonConvert.DeserializeObject<GetDateFormatsResponse>(dateFormats);
                        File.WriteAllText(fileDateFormats, JsonConvert.SerializeObject(dateFormatsResponse.DateFormats));
                    }
                }

                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile(new MapperProfileWebModels(cacheCards.Get(), provider));
                    cfg.AddProfile<MapperProfileTrackerWpf>();
                    cfg.AddProfile<MapperProfileLibOutputLogParser>();
                    cfg.AddProfile(new MapperProfileLibCardConvert(cacheCards, provider.GetService<DeckListConverter>(), eventsScheduleManager));
                });

                var mainWindow = provider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                var userId = "N/A";
                var fileAppSettings = Path.Combine(folderForConfigAndLog, "appSettings.json");
                if (File.Exists(fileAppSettings))
                {
                    try
                    {
                        var appSettings = JsonConvert.DeserializeObject<ConfigModelApp>(File.ReadAllText(fileAppSettings));
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
    }
}
