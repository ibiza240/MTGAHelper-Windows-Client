using Microsoft.Extensions.DependencyInjection;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.Config;
using System.Collections.Generic;

namespace MTGAHelper.Entity.IoC
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection RegisterServicesEntity(this IServiceCollection services)
        {
            return services
                .AddSingleton<CacheSingleton<ICollection<ConfigModelDeck>>>()
                .AddSingleton<CacheSingleton<ICollection<Card>>>()
                .AddSingleton<CacheDictionarySingleton<string, ConfigModelUser>>()
                .AddSingleton<CacheSingleton<DraftRatings>>()
                .AddTransient<RawDeckConverter>()
                .AddTransient<LogSplitter>()
                .AddTransient<UtilColors>();
        }
    }
}
