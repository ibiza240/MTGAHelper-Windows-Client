using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;

namespace MTGAHelper.Web.Models.IoC
{
    public class AutoMapperIntToCardArtConverter : IValueConverter<int, string>
    {
        private readonly Dictionary<int, Card> cards;

        public AutoMapperIntToCardArtConverter(CacheSingleton<Dictionary<int, Card>> cache)
        {
            this.cards = cache.Get();
        }

        public string Convert(int sourceMember, ResolutionContext context)
        {
            if (cards.ContainsKey(sourceMember) == false)
                return "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on";

            return cards[sourceMember].imageArtUrl;
        }
    }
}