using AutoMapper;
using MTGAHelper.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.Response.Deck
{
    public class DeckResponse
    {
        public DeckDto Deck { get; }

        public DeckResponse(DeckDto deck)
        {
            Deck = deck;
        }

        public static DeckResponse FromDeckInfo(
            DeckTrackedDetails deckInfo,
            UtilManaCurve utilManaCurve,
            IMapper mapper)
        {
            var hCards = new Dictionary<bool, HashSet<string>>
            {
                { true, new HashSet<string>() },
                { false, new HashSet<string>() },
            };

            var cards = deckInfo.Deck.Cards.All
                .OrderBy(i => i.Card.type.Contains("Land") ? 0 : 1)
                .ThenBy(i => i.Card.GetSimpleType())
                .ThenBy(i => i.Card.cmc)
                .ThenBy(i => i.Card.name)
                .Select(i =>
                {
                    //if (i.Card.name.Contains("eace")) System.Diagnostics.Debugger.Break();

                    var card = mapper.Map<DeckCardDto>(i);

                    card.NbMissing =
                        hCards[i.Zone == DeckCardZoneEnum.Sideboard].Contains(i.Card.name) ? 0 : (deckInfo.CardsRequired.ByCard.ContainsKey(i.Card.name) ?
                        (i.Zone == DeckCardZoneEnum.Sideboard ? deckInfo.CardsRequired.ByCard[i.Card.name].NbMissingSideboard : deckInfo.CardsRequired.ByCard[i.Card.name].NbMissingMain) : 0);

                    if (hCards[i.Zone == DeckCardZoneEnum.Sideboard].Contains(i.Card.name) == false) hCards[i.Zone == DeckCardZoneEnum.Sideboard].Add(i.Card.name);

                    return card;
                }).ToArray();

            var compareResults = deckInfo.CompareResult.GetModelSummary()
                .Select(i => new DeckCompareResultDto
                {
                    Set = i.Key,
                    NbMissing = i.Value.Sum(x => x.NbMissing),
                    MissingWeightTotal = i.Value.Sum(x => x.MissingWeight),
                })
                .OrderByDescending(i => i.MissingWeightTotal)
                .ThenByDescending(i => i.NbMissing)
                .ToArray();

            var deck = new DeckDto
            {
                OriginalDeckId = deckInfo.OriginalDeckId,
                Id = deckInfo.Deck.Id,
                Hash = Fnv1aHasher.To32BitFnv1aHash(deckInfo.OriginalDeckId),
                Name = deckInfo.Deck.Name,
                Url = deckInfo.Config.Url,
                ScraperTypeId = deckInfo.Deck.ScraperType.Id,
                CardsMain = cards.Where(i => i.Zone == DeckCardZoneEnum.Deck.ToString()).ToArray(),
                CardsNotMainByZone = cards
                    .Where(i => i.Zone != DeckCardZoneEnum.Deck.ToString())
                    .GroupBy(i => i.Zone)
                    .Select(i => new KeyValuePair<string, DeckCardDto[]>(i.Key, i.ToArray()))
                    .ToArray(),
                MtgaImportFormat = deckInfo.MtgaImportFormat,
                CompareResults = compareResults,
                ManaCurve = utilManaCurve.CalculateManaCurve(deckInfo.Deck.Cards.QuickCardsMain.Values
                    .Cast<CardWithAmount>().ToArray())
            };

            //if (deckInfo.Deck is DeckAverageArchetype archetype)
            //{
            //    throw new Exception("TO REVISE");
            //    deck.CardsMainOther = archetype.CardsMainOther.Select(i => new DeckCardDto
            //    {
            //        Name = i.name,
            //        Rarity = i.GetRarityEnum(false).ToString(),
            //        Type = i.GetSimpleType(),
            //        NbMissing = i.NbMissing,
            //        ImageCardUrl = i.imageCardUrl,//.images["normal"]
            //        //ImageArtUrl = i.imageArtUrl,//.images["normal"]
            //    }).ToArray();
            //}

            return new DeckResponse(deck);
        }
    }
}