using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Tracker.WPF.Views;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection RegisterServicesApp(this IServiceCollection services, IConfiguration configApp)
        {
            return services
                .Configure<ConfigModelApp>(configApp)
                .Configure<Lib.Config.ConfigModelApp>(configApp.GetSection("configApp"))
                .AddOptions()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowVM>()
                .AddSingleton<InMatchTrackerStateVM>()
                .AddSingleton<ProcessMonitor>()
                .AddSingleton<FileMonitor>()
                .AddSingleton<ServerApiCaller>()

                //.AddTransient<Ready>()
                //.AddTransient<StatusBarTop>()
                //.AddTransient<OptionsWindow>()
                .AddTransient<LogFileZipper>()
                .AddTransient<StartupShortcutManager>()
                .AddTransient<DraftHelper>()
                .AddTransient<StatusBlinker>()
                .AddTransient<MtgaResourcesLocator>()
                .AddTransient<ExternalProviderTokenManager>()
                //.AddTransient<LogProcessor>()
                ;
        }
    }
}
