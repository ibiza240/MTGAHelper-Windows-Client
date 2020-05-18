using MTGAHelper.Tracker.DraftHelper.Shared.Services;
using SimpleInjector;

namespace MTGAHelper.Tracker.DraftHelper.Shared.IoC
{
    public static class IServiceCollectionExtension
    {
        public static Container RegisterServicesDrafHelperShared(this Container container)
        {
            container.RegisterSingleton<DraftRatingsLoader>();

            return container;
        }
    }
}
