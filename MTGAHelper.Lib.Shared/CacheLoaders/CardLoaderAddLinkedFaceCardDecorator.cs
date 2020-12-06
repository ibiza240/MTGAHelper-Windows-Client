using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity;

namespace MTGAHelper.Lib.CacheLoaders
{
    public class CardLoaderAddLinkedFaceCardDecorator : ICacheLoader<Dictionary<int, Card>>
    {
        private readonly ICacheLoader<Dictionary<int, Card>> decoratee;

        public CardLoaderAddLinkedFaceCardDecorator(ICacheLoader<Dictionary<int, Card>> decoratee)
        {
            this.decoratee = decoratee;
        }

        public Dictionary<int, Card> LoadData()
        {
            var allCardsDict = decoratee.LoadData();

            foreach (var card in allCardsDict.Values.Where(c => c.LinkedCardGrpId > 0))
            {
                card.SetLinkedCard(allCardsDict.GetValueOrDefault(card.LinkedCardGrpId));
            }

            return allCardsDict;
        }
    }
}
