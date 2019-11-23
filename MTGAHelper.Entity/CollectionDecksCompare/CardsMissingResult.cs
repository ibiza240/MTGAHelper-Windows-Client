using MTGAHelper.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.CollectionDecksCompare
{
    public class CardsMissingResult
    {
        ICollection<CardRequiredInfo> computedData;

        public Dictionary<string, CardRequiredInfoByDeck> ByDeck { get; private set; } = new Dictionary<string, CardRequiredInfoByDeck>();

        public Dictionary<string, CardRequiredInfoByCard> ByCard { get; private set; } = new Dictionary<string, CardRequiredInfoByCard>();

        public CardsMissingResult()
        {
        }

        public CardsMissingResult(ICollection<CardRequiredInfo> computedData, Dictionary<string, bool> decksTracked)
        {
            //var test = computedData.FirstOrDefault(i => i.Card.name == "Teferi, Hero of Dominaria");
            this.computedData = computedData;

            ByDeck = computedData
                .Where(i => i.IsForAverageArchetypeOthersInMain == false)
                .GroupBy(i => i.DeckId)
                .ToDictionary(i => i.Key, i => new CardRequiredInfoByDeck(i.Key, i, decksTracked));

            ByCard = computedData
                .Where(i => i.IsForAverageArchetypeOthersInMain == false)
                .GroupBy(i => i.Card.name)
                .ToDictionary(i => i.Key, i => new CardRequiredInfoByCard(i, decksTracked));

            //var test2 = ByCard.FirstOrDefault(i => i.Key.name == "Teferi, Hero of Dominaria");
        }

        public Dictionary<string, InfoCardMissingSummary[]> GetModelSummary()
        {
            var ret = ByCard
                //.Where(i => i.Value.Card.setAndInBooster != Card.NOTINBOOSTER)
                .Where(i => i.Value.Card.isCollectible)
                .Where(i => i.Value.NbMissing > 0)
                //.GroupBy(i => i.Value.Card.setAndInBooster)
                .GroupBy(i => i.Value.Card.set)
                .OrderByDescending(i => i.Sum(x => x.Value.MissingWeight))
                .ToDictionary(i => i.Key, x => x
                    .OrderBy(i => i.Value.Card.GetRarityEnum(true))
                    .GroupBy(i => i.Value.Card.GetRarityEnum(true)).Select(i => new InfoCardMissingSummary
                    {
                        Set = x.Key,
                        //NotInBooster = x.Key == Card.NOTINBOOSTER,
                        Rarity = i.Key,
                        NbMissing = i.Sum(c => c.Value.NbMissing),
                        MissingWeight = i.Sum(c => c.Value.MissingWeight)
                    }).ToArray());
            return ret;
        }

        public CardMissingDetailsModel[] GetModelDetails()
        {
            var ret = ByCard
                .Where(i => i.Value.NbMissing > 0)
                //.Where(i =>/* i.Value.Card.type.Contains("Land") ||*/ i.Value.MissingWeight != 0)
                .Select(i => new CardMissingDetailsModel
                {
                    CardName = i.Value.Card.name,
                    Set = i.Value.Card.set,
                    //SetId = i.Key.number,
                    NotInBooster = i.Value.Card.notInBooster,
                    ImageCardUrl = i.Value.Card.imageCardUrl,//i.Key.images["normal"],
                    Rarity = i.Value.Card.GetRarityEnum(true).ToString(),
                    Type = i.Value.Card.type,
                    NbOwned = i.Value.NbOwned,
                    NbMissing = i.Value.NbMissing,
                    MissingWeight = i.Value.MissingWeight,
                    NbDecks = i.Value.NbDecks,
                    NbAvgPerDeck = System.Math.Round((double)i.Value.ByDeck.Sum(x => x.Value.NbRequired) / i.Value.ByDeck.Count, 2),
                })
                .OrderByDescending(i => i.MissingWeight)
                .ThenBy(i => i.CardName)
                .ToArray();
            return ret;
        }

        public InfoCardInDeck[] GetModelMissingCardsAllDecks()
        {
            return computedData
                //.Where(i => i.IsForAverageArchetypeOthersInMain == false)
                .Where(i => i.NbMissing > 0)
                .GroupBy(i => new { i.DeckName, i.Card })
                .Select(i => new InfoCardInDeck
                {
                    Set = i.Key.Card.set,
                    CardName = i.Key.Card.name,
                    Rarity = i.Key.Card.rarity,
                    Type = i.Key.Card.type,
                    Deck = i.Key.DeckName,
                    NbMissingMain = i.Where(x => x.IsSideboard == false).Sum(x => x.NbMissing),
                    NbMissingSideboard = i.Where(x => x.IsSideboard).Sum(x => x.NbMissing),
                })
                .OrderBy(i => i.CardName)
                .ThenBy(i => i.Deck)
                .ToArray();
        }

        public Dictionary<RarityEnum, int> GetWildcardsMissing(string deckId, bool isSideboard, bool splitRareLands)
        {
            if (ByDeck.ContainsKey(deckId) == false)
            {
                return new Dictionary<RarityEnum, int>();
            }

            var ret = new Dictionary<RarityEnum, int>();
            var cards = ByDeck[deckId].ByCard;

            ret.Add(RarityEnum.Mythic,
                cards
                    .Where(i => i.Value.Card.GetRarityEnum() == RarityEnum.Mythic)
                    .Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));

            var rares = cards.Where(i => i.Value.Card.GetRarityEnum() == RarityEnum.Rare);
            if (splitRareLands)
            {
                ret.Add(RarityEnum.RareNonLand,
                    rares
                        .Where(i => i.Value.Card.type.Contains("Land") == false)
                        .Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));

                ret.Add(RarityEnum.RareLand,
                    rares
                        .Where(i => i.Value.Card.type.Contains("Land") == true)
                        .Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));
            }
            else
            {
                ret.Add(RarityEnum.Rare,
                    rares.Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));
            }

            ret.Add(RarityEnum.Uncommon,
                cards
                    .Where(i => i.Value.Card.GetRarityEnum() == RarityEnum.Uncommon)
                    .Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));

            ret.Add(RarityEnum.Common,
                cards
                    .Where(i => i.Value.Card.GetRarityEnum() == RarityEnum.Common)
                    .Sum(i => isSideboard ? i.Value.NbMissingSideboard : i.Value.NbMissingMain));

            return ret;
        }
    }
}
