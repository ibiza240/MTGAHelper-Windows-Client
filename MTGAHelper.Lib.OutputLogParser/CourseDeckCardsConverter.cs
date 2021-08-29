using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class CourseDeckCardsConverter : IValueConverter<CourseDeckRaw, ICollection<DeckCardRaw>>
    {
        private readonly DeckListConverter deckListConverter;

        public CourseDeckCardsConverter(DeckListConverter deckListConverter)
        {
            this.deckListConverter = deckListConverter;
        }

        public ICollection<DeckCardRaw> Convert(CourseDeckRaw src, ResolutionContext context)
        {
            var cards = deckListConverter.ConvertSimple(src.mainDeck)
                .Select(i => new DeckCardRaw
                {
                    GrpId = i.Key,
                    Amount = i.Value,
                    Zone = DeckCardZoneEnum.Deck,
                })
                .Union(deckListConverter.ConvertSimple(src.sideboard).Select(i => new DeckCardRaw
                {
                    GrpId = i.Key,
                    Amount = i.Value,
                    Zone = DeckCardZoneEnum.Sideboard,
                }
                ))
                .ToList();

            if (src.commandZoneGRPIds?.Count > 0)
                cards.Add(new DeckCardRaw
                {
                    GrpId = src.commandZoneGRPIds.First(),
                    Amount = 1,
                    Zone = DeckCardZoneEnum.Commander,
                });

            if (src.companionGRPId != 0)
                cards.Add(new DeckCardRaw
                {
                    GrpId = src.companionGRPId,
                    Amount = 1,
                    Zone = DeckCardZoneEnum.Companion,
                });

            return cards;
        }
    }
}