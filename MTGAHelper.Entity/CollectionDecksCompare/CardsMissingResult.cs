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
            //return computedData
            //    .Where(i => i.NbMissing > 0)
            //    .GroupBy(i => i.Card.set)
            //    .ToDictionary(i => i.Key, x => x.GroupBy(i => i.Card.GetRarityEnum(true)).Select(i => new InfoCardMissingSummary {
            //        Set = x.Key,
            //        Rarity = i.Key,
            //        NbMissing = i.Sum(c => c.NbMissing),
            //        MissingWeight = i.Sum(c => c.MissingWeight)
            //    }).ToArray());

            //var test = ByDeck.First().Value.ByCard.First().Value;

            return ByCard
                .Where(i => i.Value.NbMissing > 0)
                .GroupBy(i => i.Value.Card.setConsideringCraftedOnly)
                .ToDictionary(i => i.Key, x => x
                    .OrderBy(i => i.Value.Card.GetRarityEnum(true))
                    .GroupBy(i => i.Value.Card.GetRarityEnum(true)).Select(i => new InfoCardMissingSummary
                    {
                        Set = x.Key,
                        CraftedOnly = x.Key == Card.CRAFTEDONLY,
                        Rarity = i.Key,
                        NbMissing = i.Sum(c => c.Value.NbMissing),
                        MissingWeight = i.Sum(c => c.Value.MissingWeight)
                    }).ToArray());

            //var ret = new List<InfoCardMissingSummary>();

            //var gBySet = ByCard
            //    .GroupBy(i => i.Key.set)
            //    .OrderByDescending(i => i.Sum(x => x.Value.MissingWeight));

            //foreach (var g in gBySet)
            //{
            //    ret.Add(new InfoCardMissingSummary
            //    {
            //        Label = g.Key,
            //        IsGrouping = true,
            //        NbMissing = g.Sum(i => i.Value.NbMissing),
            //        MissingWeightSum = g.Sum(i => i.Value.MissingWeight)
            //    });

            //    var g1 = g
            //        //.GroupBy(i => $"{i.Key.rarity}{(i.Key.rarity == "Rare" ? $" ({(i.Key.type.Contains("Land") ? "Land" : "Nonland")})" : "")}")
            //        .GroupBy(i => i.Key.GetRarityEnum(true))
            //        .Where(i => i.Sum(x => x.Value.NbMissing) > 0)
            //        ////.OrderByDescending(i => i.Sum(x => x.Value.Priority));
            //        //.OrderBy(i => Entity.Util.GetOrderingByRarity(i.Key));
            //        .OrderBy(i => i.Key);

            //    foreach (var g2 in g1)
            //    {
            //        ret.Add(new InfoCardMissingSummary
            //        {
            //            Label = $"   {g2.Key}",
            //            IsGrouping = false,
            //            NbMissing = g2.Sum(i => i.Value.NbMissing),
            //            MissingWeightSum = g2.Sum(i => i.Value.MissingWeight)
            //        });
            //    }
            //}

            //return ret.ToArray();
        }

        public CardMissingDetailsModel[] GetModelDetails()
        {
            return ByCard
                .Where(i => i.Value.NbMissing > 0)
                .Where(i => i.Value.Card.type.Contains("Land") || i.Value.MissingWeight != 0)
                .Select(i => new CardMissingDetailsModel
                {
                    CardName = i.Value.Card.name,
                    Set = i.Value.Card.set,
                    //SetId = i.Key.number,
                    CraftedOnly = i.Value.Card.craftedOnly,
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
                return new Dictionary<RarityEnum, int>
                {
                };
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
