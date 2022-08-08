using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetDeck;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class DeckCardsConverter : ITypeConverter<GetDeckRaw, ICollection<DeckCardRaw>>
    {
        public ICollection<DeckCardRaw> Convert(GetDeckRaw source, ICollection<DeckCardRaw> destination, ResolutionContext context)
        {
            var main = source.MainDeck
                .Select(i => new DeckCardRaw
                {
                    Zone = DeckCardZoneEnum.Deck,
                    Amount = i.quantity,
                    GrpId = i.cardId,
                });

            var sideboard = source.Sideboard
                .Select(i => new DeckCardRaw
                {
                    Zone = DeckCardZoneEnum.Deck,
                    Amount = i.quantity,
                    GrpId = i.cardId,
                });

            var companions = source.Companions
                .Select(i => new DeckCardRaw
                {
                    Zone = DeckCardZoneEnum.Deck,
                    Amount = i.quantity,
                    GrpId = i.cardId,
                });

            var commander = source.CommandZone
                .Select(i => new DeckCardRaw
                {
                    Zone = DeckCardZoneEnum.Deck,
                    Amount = i.quantity,
                    GrpId = i.cardId,
                });

            return main
                .Union(sideboard)
                .Union(commander)
                .Union(companions)
                .ToArray();
        }
    }
}