using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;

namespace MTGAHelper.Web.Models.IoC
{
    public class AutoMapperCollationToSetConverter : IValueConverter<int, string>
    {
        private readonly CacheSingleton<Dictionary<int, Set>> cache;

        public AutoMapperCollationToSetConverter(CacheSingleton<Dictionary<int, Set>> cacheSetsByCollation)
        {
            this.cache = cacheSetsByCollation;
        }

        public string Convert(int sourceMember, ResolutionContext context)
        {
            var setsByCollation = cache.Get();
            return setsByCollation[sourceMember].Code;
        }
    }
}