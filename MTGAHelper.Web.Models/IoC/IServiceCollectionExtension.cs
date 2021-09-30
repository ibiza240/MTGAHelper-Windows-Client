using SimpleInjector;
using System.Linq;

namespace MTGAHelper.Web.Models.IoC
{
    public static class IServiceCollectionExtension
    {
        public static Container RegisterServicesWebModels(this Container container)
        {
            var assembly = typeof(IServiceCollectionExtension).Assembly;

            var automapperConverters = assembly.GetExportedTypes()
                .Where(type => type.Name.StartsWith("AutoMapper")
                               && type.Name.EndsWith("Converter"));

            foreach (var impl in automapperConverters)
            {
                container.RegisterSingleton(impl, impl);
            }

            container.Collection.Append<AutoMapper.Profile, MapperProfileWebModels>(Lifestyle.Singleton);
            container.RegisterSingleton<UtilManaCurve>();

            return container;
        }
    }
}