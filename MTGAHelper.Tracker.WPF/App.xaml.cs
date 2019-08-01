using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgaHelper.Web.Models.IoC;
using MTGAHelper.Entity;
using MTGAHelper.Entity.IoC;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.IoC;
using MTGAHelper.Tracker.WPF.Views;
using MTGAHelper.Web.UI.IoC;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF
{
    public partial class App : Application
    {
        IConfiguration configuration;
        IServiceProvider provider;

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            var folderForConfigAndLog = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#else
            var folderForConfigAndLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
#endif

            configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(folderForConfigAndLog, "appsettings.json"), optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File(
                    Path.Combine(folderForConfigAndLog, "log-.txt"),
                    rollingInterval:RollingInterval.Month,
                    outputTemplate: "{Timestamp:dd HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Write(LogEventLevel.Information, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .RegisterServicesApp(configuration)
                .RegisterServicesWebModels()
                .RegisterServicesEntity();

            provider = serviceCollection.BuildServiceProvider();

            var cacheCards = provider.GetService<CacheSingleton<ICollection<Card>>>();
            cacheCards.PopulateIfNotSet(() => new Card[0]);

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new MapperProfileWebModels(new Card[0], provider));
                cfg.AddProfile<MapperProfileTrackerWpf>();
            });

            var mainWindow = provider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
