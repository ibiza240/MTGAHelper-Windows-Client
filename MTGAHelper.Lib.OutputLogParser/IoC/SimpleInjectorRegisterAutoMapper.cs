using AutoMapper;
using AutoMapper.Configuration;
using SimpleInjector;

namespace MTGAHelper.Lib.OutputLogParser.IoC
{
    public static class SimpleInjectorRegisterAutoMapper
    {
        public static Container RegisterMapperConfig(this Container c)
        {
            MapperConfigurationExpression BuildMapperConfig()
            {
                var cfg = new MapperConfigurationExpression();
                foreach (var profile in c.GetAllInstances<Profile>())
                {
                    cfg.AddProfile(profile);
                }
                return cfg;
            }

            c.RegisterSingleton(BuildMapperConfig);
            c.RegisterSingleton<IMapper>(() =>
                new MapperConfiguration(c.GetInstance<MapperConfigurationExpression>())
                    .CreateMapper());

            return c;
        }
    }
}
