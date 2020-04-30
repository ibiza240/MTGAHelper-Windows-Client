using System.Collections.Generic;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Web.UI.Shared;
using SimpleInjector;

namespace MTGAHelper.Entity.IoC
{
    public static class SimpleInjectorRegistrations
    {
        public static Container RegisterServicesEntity(this Container container)
        {
            container.RegisterConditional(
                typeof(CacheSingleton<>),
                typeof(CacheSingleton<>),
                Lifestyle.Singleton,
                c => !c.Handled);

            container.RegisterSingleton<ICollection<Card>>(() => container.GetInstance<CacheSingleton<Dictionary<int, Card>>>().Get().Values);
            container.Collection.Append<AutoMapper.Profile, MapperProfileEntity>(Lifestyle.Singleton);
            container.RegisterSingleton<AutoMapperGrpIdToCardConverter>();
            container.RegisterSingleton<Util>();
            container.RegisterSingleton<PasswordHasher>();
            container.RegisterSingleton<UtilColors>();
            container.RegisterSingleton<RawDeckConverter>();
            container.RegisterSingleton<LogSplitter>();

            return container;
        }
    }
}
