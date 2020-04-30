using AutoMapper;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Tracker.WPF.Views;
using SimpleInjector;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public static class ServiceCollectionExtension
    {
        public static Container RegisterDataPath(this Container container, string folderDataPath)
        {
            container.RegisterInstance<Lib.Config.IDataPath>(new Lib.Config.DataFolderPath(folderDataPath));
            return container;
        }

        public static Container RegisterServicesApp(this Container container, ConfigModel configApp)
        {
            container.RegisterInstance(configApp);
            container.Register<MainWindow>();
            container.RegisterSingleton<MainWindowVM>();
            container.RegisterSingleton<InMatchTrackerStateVM>();
            container.RegisterSingleton<ProcessMonitor>();
            container.RegisterSingleton<FileMonitor>();
            container.RegisterSingleton<ServerApiCaller>();
            container.RegisterSingleton<StatusBlinker>();
            container.Collection.Append<Profile, MapperProfileTrackerWpf>(Lifestyle.Singleton);
            container.RegisterSingleton<BorderGradientCalculator>();
            container.RegisterSingleton<LogFileZipper>();
            container.RegisterSingleton<StartupShortcutManager>();
            container.RegisterSingleton<DraftCardsPicker>();
            container.RegisterSingleton<MtgaResourcesLocator>();
            container.Register<ExternalProviderTokenManager>();
            container.Register<ScreenshotTaker>();
            container.Register<DraftHelperRunner>();

            return container;
        }
    }
}
