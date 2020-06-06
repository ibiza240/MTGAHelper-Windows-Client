using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperIntToCardArtConverter : IValueConverter<int, string>
    {
        readonly Dictionary<int, Card> cards;

        public AutoMapperIntToCardArtConverter(CacheSingleton<Dictionary<int, Card>> cache)
        {
            this.cards = cache.Get();
        }

        public string Convert(int sourceMember, ResolutionContext context)
        {
            return cards[sourceMember].imageArtUrl;
        }
    }
}
