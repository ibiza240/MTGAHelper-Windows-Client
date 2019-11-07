using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public enum enumLinkedFace
    {
        None = 0,
        DFC_Front = 1,
        DFC_Back = 2,
        MeldCard = 3,
        MeldedPermanent = 4,
        SplitCard = 5,
        SplitHalf = 6,
        AdventureAdventure = 7,
        AdventureCreature = 8,
    }

    public class RawDeckConverter
    {
        public Dictionary<int, int> ResultRaw { get; private set; }

        Dictionary<int, Card> allCards;

        public RawDeckConverter Init(Dictionary<int, Card> allCards)
        {
            this.allCards = allCards;
            return this;
        }

        public ICollection<CardWithAmount> LoadCollection(string collectionJson)
        {
            ResultRaw = JsonConvert.DeserializeObject<Dictionary<int, int>>(collectionJson);
            return LoadCollection(ResultRaw);
        }

        public ICollection<CardWithAmount> LoadCollection(Dictionary<int, int> info)
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
