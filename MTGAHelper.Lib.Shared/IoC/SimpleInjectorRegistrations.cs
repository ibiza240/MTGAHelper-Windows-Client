using System.Collections.Generic;
using MTGAHelper.Entity;
using MTGAHelper.Lib.CacheLoaders;
using SimpleInjector;

namespace MTGAHelper.Lib.IoC
{
    public static class SimpleInjectorRegistrations
    {
        public static Container RegisterServicesShared(this Container container)
        {
            container.RegisterConditional(
                typeof(CacheSingleton<>),
                typeof(CacheSingleton<>),
                Lifestyle.Singleton,
                c => !c.Handled);

            container.RegisterSingleton<ICollection<Card>>(() => container.GetInstance<CacheSingleton<Dictionary<int, Card>>>().Get().Values);
            container.RegisterSingleton(() => container.GetInstance<CacheSingleton<Dictionary<int, Card>>>().Get());
            container.Collection.Append<AutoMapper.Profile, MapperProfileEntity>(Lifestyle.Singleton);
            container.RegisterSingleton<AutoMapperGrpIdToCardConverter>();
            container.RegisterSingleton<Util>();
            container.RegisterSingleton<PasswordHasher>();
            container.RegisterSingleton<UtilColors>();
            container.RegisterSingleton<RawDeckConverter>();

            return container;
        }

        public static Container RegisterFileLoadersShared(this Container container)
        {
            container.RegisterSingleton<ICacheLoader<Dictionary<int, Set>>, CacheLoaderSets>();
            container.RegisterSingleton<ICacheLoader<Dictionary<string, DraftRatings>>, CacheLoaderDraftRatings>();
            container.RegisterSingleton<ICacheLoader<Dictionary<int, Card>>, CacheLoaderAllCards>();

            return container;
        }
    }
}
