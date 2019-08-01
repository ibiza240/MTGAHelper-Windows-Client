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
                .AddOptions()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowVM>()
                .AddSingleton<ProcessMonitor>()
                .AddSingleton<FileMonitor>()

                //.AddTransient<Ready>()
                //.AddTransient<StatusBarTop>()
                .AddTransient<OptionsWindow>()
                .AddTransient<LogFileZipper>()
                .AddTransient<ServerApiCaller>()
                .AddTransient<StartupShortcutManager>()
                .AddTransient<DraftHelper>()
                .AddTransient<StatusBlinker>()
                .AddTransient<MtgaResourcesLocator>();
        }
    }
}
