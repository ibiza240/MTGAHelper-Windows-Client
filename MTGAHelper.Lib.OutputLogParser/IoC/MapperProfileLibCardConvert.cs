using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.OutputLogProgress;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.IoC
{
    public class MapperProfileLibCardConvert : Profile
    {
        public MapperProfileLibCardConvert(
            AutoMapperEventNameToTypeConverter eventNameToType,
            DeckListConverter deckListConverter,
            CourseDeckCardsConverter courseDeckCardsConverter,
            EventSetDeckToCardsConverter eventSetDeckToCardsConverter
            )
        {
            CreateMap<GameProgress, GameDetail>()
                .ForMember(i => i.OpponentCardsSeen, i => i.MapFrom(x => x.OpponentCardsSeenByInstanceId.Values
                    .GroupBy(grpId => grpId)
                    .ToDictionary(y => y.Key, y => y.Count())))
                .ForMember(i => i.CardTransfers, i => i.MapFrom(x => x.CardTransfersByTurn.SelectMany(y => y.Value).ToArray()));

            CreateMap<CardForTurn, CardTurnAction>();

            // To map all the opponent fields
            CreateMap<ConnectingToMatchIdResult, MatchResult>()
                .ForMember(i => i.StartDateTime, i => i.MapFrom(x => x.LogDateTime))
                //!!!
                //.ForMember(i => i.EventName, i => i.MapFrom(x => x.Raw.payload.eventId))
                //.ForMember(i => i.EventType, i => i.ConvertUsing(eventNameToType, x => x.Raw.payload.eventId))
                //.ForMember(i => i.Opponent, i => i.MapFrom(x => x.Raw.payload))
                //!!!
                .ForMember(m => m.Opponent, o => o.Ignore())
                .ForMember(m => m.Games, o => o.Ignore())
                .ForMember(m => m.DeckUsed, o => o.Ignore())
                .ForMember(m => m.Outcome, o => o.Ignore());

            //CreateMap<CourseDeckRaw, MtgaDeck>()
            CreateMap<Entity.OutputLogParsing.CourseDeckRaw, ConfigModelRawDeck>()
                .ForMember(i => i.DeckTileId, i => i.MapFrom(x => x.deckTileId ?? default(int)))
                //.ForMember(i => i.CardCommander, i => i.MapFrom(x => x.commandZoneGRPIds.FirstOrDefault()))
                //.ForMember(i => i.CardsMain, i => i.ConvertUsing(deckListConverter, x => x.mainDeck))
                //.ForMember(i => i.CardsSideboard, i => i.ConvertUsing(deckListConverter, x => x.sideboard))

                .ForMember(i => i.Cards, i => i.ConvertUsing(courseDeckCardsConverter, x => x))

                .ForMember(m => m.ArchetypeId, o => o.Ignore()) // todo Bruno?
                .ForMember(m => m.CardsMain, o => o.Ignore()) // Obsolete
                .ForMember(m => m.CardsSideboard, o => o.Ignore()) // Obsolete
                .ForMember(m => m.CardCommander, o => o.Ignore()) // Obsolete
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            CreateMap<EventSetDeckRaw, ConfigModelRawDeck>()
                .ForMember(i => i.Id, i => i.MapFrom(o => o.Summary.DeckId))
                .ForMember(i => i.DeckTileId, o => o.Ignore())
                .ForMember(i => i.LastUpdated, o => o.Ignore())
                .ForMember(i => i.Name, i => i.MapFrom(o => o.Summary.Name))
                .ForMember(i => i.Format, o => o.Ignore())
                .ForMember(m => m.ArchetypeId, o => o.Ignore()) // todo Bruno?
                .ForMember(m => m.CardsMain, o => o.Ignore()) // Obsolete
                .ForMember(m => m.CardsSideboard, o => o.Ignore()) // Obsolete
                .ForMember(m => m.CardCommander, o => o.Ignore()) // Obsolete
                .ForMember(i => i.Cards, i => i.ConvertUsing(eventSetDeckToCardsConverter, x => x))
                .ForMember(i => i.Description, o => o.Ignore())
                .ForMember(i => i.CardBack, o => o.Ignore());

            ////CreateMap<GetDeckListResultDeckRaw, MtgaDeck>()
            //CreateMap<GetDeckListResultDeckRaw, ConfigModelRawDeck>()
            //    .ForMember(i => i.CardsMain, i => i.MapFrom(x => deckListConverter.ConvertSimple(x.mainDeck)))
            //    .ForMember(i => i.CardsSideboard, i => i.MapFrom(x => deckListConverter.ConvertSimple(x.sideboard)))
            //    .ForMember(i => i.DeckTileId, i => i.MapFrom(x => x.deckTileId ?? default(int)));
        }
    }
}