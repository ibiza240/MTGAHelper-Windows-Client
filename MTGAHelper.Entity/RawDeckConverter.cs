using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.Cache;

namespace MTGAHelper.Entity
{
    // TODO: move out of Entity
    public class RawDeckConverter
    {
        readonly Dictionary<int, Card> allCards;

        public RawDeckConverter(CacheSingleton<Dictionary<int, Card>> allCards)
        {
            this.allCards = allCards.Get();
        }

        public ICollection<CardWithAmount> LoadCollection(IReadOnlyDictionary<int, int> info)
        {
            if (info == null)
                return new CardWithAmount[0];

            //if (allCards == null)
            //    System.Diagnostics.Debugger.Break();

            var grpIdsKnown = allCards.Keys;
            var grpIdsNotFound = info.Keys.Where(i => grpIdsKnown.Contains(i) == false).ToArray();

            var cards = new Dictionary<string, CardWithAmount>();
            foreach (var kv in info.Where(i => grpIdsNotFound.Contains(i.Key) == false))
            {
                try
                {

                    var cardMapping = allCards[kv.Key];
                    var card = cardMapping;
                    cards.Add(card.grpId.ToString(), new CardWithAmount(card, kv.Value));
                }
                catch (Exception ex)
                {
                    throw new CardsLoaderException($"Problem loading card {kv.Value} in collection", ex);
                }
            }

            return cards.Values
                .Where(i => i.Card.type.StartsWith("Basic Land") == false)
                .Where(i => i.Card.isToken == false)
                .Where(i => i.Card.linkedFaceType != enumLinkedFace.SplitCard)
                .Where(i => i.Card.linkedFaceType != enumLinkedFace.DFC_Front)
                .OrderBy(i => i.Card.name)
                .ToArray();
        }
    }
}
