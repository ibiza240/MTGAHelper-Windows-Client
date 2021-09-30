using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventSetDeck;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class EventSetDeckToCardsConverter : IValueConverter<EventSetDeckRaw, ICollection<DeckCardRaw>>
    {
        public EventSetDeckToCardsConverter()
        {
        }

        public ICollection<DeckCardRaw> Convert(EventSetDeckRaw src, ResolutionContext context)
        {
            var cards = src.Deck.MainDeck
                .Select(i => new DeckCardRaw
                {
                    GrpId = i.CardId,
                    Amount = i.Quantity,
                    Zone = DeckCardZoneEnum.Deck,
                })
                .Union((src.Deck.DoPreferReducedSideboard ? src.Deck.ReducedSideboard : src.Deck.Sideboard)
                    .Select(i => new DeckCardRaw
                    {
                        GrpId = i.CardId,
                        Amount = i.Quantity,
                        Zone = DeckCardZoneEnum.Deck,
                    })
                )
                ;

            // TODO: Add Commander or Companion

            return cards.ToArray();
            //var cards = deckListConverter.ConvertSimple(src.mainDeck)
            //    .Select(i => new DeckCardRaw
            //    {
            //        GrpId = i.Key,
            //        Amount = i.Value,
            //        Zone = DeckCardZoneEnum.Deck,
            //    })
            //    .Union(deckListConverter.ConvertSimple(src.sideboard).Select(i => new DeckCardRaw
            //    {
            //        GrpId = i.Key,
            //        Amount = i.Value,
            //        Zone = DeckCardZoneEnum.Sideboard,
            //    }
            //    ))
            //    .ToList();

            //if (src.commandZoneGRPIds?.Count > 0)
            //    cards.Add(new DeckCardRaw
            //    {
            //        GrpId = src.commandZoneGRPIds.First(),
            //        Amount = 1,
            //        Zone = DeckCardZoneEnum.Commander,
            //    });

            //if (src.companionGRPId != 0)
            //    cards.Add(new DeckCardRaw
            //    {
            //        GrpId = src.companionGRPId,
            //        Amount = 1,
            //        Zone = DeckCardZoneEnum.Companion,
            //    });

            //return cards;
        }
    }
}