using Microsoft.Extensions.DependencyInjection;
using MTGAHelper.Web.UI.Shared;

namespace MtgaHelper.Web.Models.IoC
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection RegisterServicesWebModels(this IServiceCollection services)
        {
            return services
                .AddTransient<AutoMapperRawDeckConverter>()
                .AddTransient<AutoMapperRawDeckToColorConverter>();
        }
    }
}
