using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;

namespace MTGAHelper.ConsoleSync.Services
{
    public class CacheLoaderCardsByApi : ICacheLoader<Dictionary<int, Card>>
    {
        readonly ServerApiCaller apiCaller;

        public CacheLoaderCardsByApi(ServerApiCaller apiCaller)
        {
            this.apiCaller = apiCaller;
        }

        public Dictionary<int, Card> LoadData()
        {
            var cards = apiCaller.GetCards();
            return cards.ToDictionary(c => c.grpId, c => c);
        }
    }
}
