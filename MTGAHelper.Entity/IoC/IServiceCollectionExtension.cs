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
                .AddSingleton<CacheSingleton<Dictionary<int, Card>>>()
                .AddSingleton<ICollection<Card>>(p => p.GetService<CacheSingleton<Dictionary<int, Card>>>().Get().Values)
                .AddSingleton<CacheSingleton<Dictionary<int, Set>>>()
                .AddSingleton<CacheSingleton<Dictionary<string, ConfigModelUser>>>()
                .AddSingleton<CacheSingleton<Dictionary<string, DraftRatings>>>()
                .AddSingleton<MapperProfileEntity>()
                .AddTransient<RawDeckConverter>()
                .AddTransient<LogSplitter>()
                .AddTransient<UtilColors>()
                .AddTransient<PasswordHasher>();
        }
    }
}
