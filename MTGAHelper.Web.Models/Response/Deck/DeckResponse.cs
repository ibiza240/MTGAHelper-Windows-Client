using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Model;
using MTGAHelper.Web.Models;
using MTGAHelper.Web.UI.Model.Response.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DeckResponse
    {
        public DeckDto Deck { get; set; }

        public DeckResponse(DeckTrackedDetails deckInfo, UtilManaCurve utilManaCurve)
        {
            var util = new Util();

            var hCards = new Dictionary<bool, HashSet<string>>
            {
                { true, new HashSet<string>() },
                { false, new HashSet<string>() },
            };

            var cards = new List<(bool IsSideboard, DeckCardDto Card)>();
            foreach (var i in deckInfo.Deck.Cards.All
                .OrderBy(i => i.Card.type.Contains("Land") ? 0 : 1)
                .ThenBy(i => i.Card.GetSimpleType())
                .ThenBy(i => i.Card.cmc)
                .ThenBy(i => i.Card.name))
            {
                //if (i.Card.name.StartsWith("Treasure"))
                //    System.Diagnostics.Debugger.Break();

                var info = (
                    i.Zone == DeckCardZoneEnum.Sideboard,
                    //new DeckCardDto
                    //{
                    //    Name = i.Card.name,
                    //    ImageCardUrl = i.Card.imageCardUrl,//.images["normal"],
                    //    //ImageArtUrl = i.Card.imageArtUrl,//.images["normal"],
                    //    Rarity = i.Card.GetRarityEnum(false).ToString(),
                    //    Type = i.Card.GetSimpleType(),
                    //    Amount = i.Amount,
                    //    NbMissing =
                    //        hCards[i.IsSideboard].Contains(i.Card.name) ? 0 : (deckInfo.CardsRequired.ByCard.ContainsKey(i.Card) ?
                    //        (i.IsSideboard ? deckInfo.CardsRequired.ByCard[i.Card].NbMissingSideboard : deckInfo.CardsRequired.ByCard[i.Card].NbMissingMain) : 0),
                    //}
                    Mapper.Map<DeckCardDto>(i)
                );

                info.Item2.NbMissing =
                    hCards[i.Zone == DeckCardZoneEnum.Sideboard].Contains(i.Card.name) ? 0 : (deckInfo.CardsRequired.ByCard.ContainsKey(i.Card.name) ?
                    (i.Zone == DeckCardZoneEnum.Sideboard ? deckInfo.CardsRequired.ByCard[i.Card.name].NbMissingSideboard : deckInfo.CardsRequired.ByCard[i.Card.name].NbMissingMain) : 0);

                //if (i.Card.name.Contains("Guildgate"))
                //    System.Diagnostics.Debugger.Break();
                if (hCards[i.Zone == DeckCardZoneEnum.Sideboard].Contains(i.Card.name) == false) hCards[i.Zone == DeckCardZoneEnum.Sideboard].Add(i.Card.name);

                cards.Add(info);
            }

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

            Deck = new DeckDto
            {
                OriginalDeckId = deckInfo.OriginalDeckId,
                Id = deckInfo.Deck.Id,
                Hash = util.To32BitFnv1aHash(deckInfo.OriginalDeckId),
                Name = deckInfo.Deck.Name,
                Url = deckInfo.Config.Url,
                ScraperTypeId = deckInfo.Deck.ScraperType.Id,
                CardsMain = cards.Where(i => i.IsSideboard == false).Select(i => i.Card).ToArray(),
                CardsSideboard = cards.Where(i => i.IsSideboard).Select(i => i.Card).ToArray(),
                MtgaImportFormat = deckInfo.MtgaImportFormat,
                CompareResults = compareResults,
                ManaCurve = utilManaCurve.CalculateManaCurve(deckInfo.Deck.Cards.QuickCardsMain.Values.Cast<CardWithAmount>().ToArray())
            };

            if (deckInfo.Deck is DeckAverageArchetype)
            {
                throw new Exception("TO REVISE");
                Deck.CardsMainOther = ((DeckAverageArchetype)deckInfo.Deck).CardsMainOther.Select(i => new DeckCardDto
                {
                    Name = i.name,
                    Rarity = i.GetRarityEnum(false).ToString(),
                    Type = i.GetSimpleType(),
                    NbMissing = i.NbMissing,
                    ImageCardUrl = i.imageCardUrl,//.images["normal"]
                    //ImageArtUrl = i.imageArtUrl,//.images["normal"]
                }).ToArray();
            }
        }
    }
}
