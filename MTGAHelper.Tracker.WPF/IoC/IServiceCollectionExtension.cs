using AutoMapper;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using SimpleInjector;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public static class ServiceCollectionExtension
    {
        public static Container RegisterDataPath(this Container container, string folderDataPath)
        {
            container.RegisterInstance<IDataPath>(new DataFolderPath(folderDataPath));
            return container;
        }

        public static Container RegisterServicesApp(this Container container, ConfigModel configApp)
        {
            var registration = Lifestyle.Transient.CreateRegistration(typeof(MainWindowVM), container);
            container.AddRegistration(typeof(MainWindowVM), registration);
            // disable this diagnostic. We (for now) don't want to deal with making all dependencies Singleton.
            // we might in the future make this a scoped lifetime and enable LoosenedLifestyleMismatchBehavior for the container. This works great for now though.
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "This VM lives as long as the application");

            container.RegisterInstance(configApp);
            container.RegisterSingleton<DraftingVM>();
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
            container.RegisterSingleton<CardThumbnailDownloader>();
            //container.RegisterSingleton<ISupporterChecker, SupporterChecker>();
            container.Register<ExternalProviderTokenManager>();
            container.Register<DraftHelperRunner>();
            //container.Register<CacheLoaderConfigResolutions>();

            return container;
        }
    }
}
