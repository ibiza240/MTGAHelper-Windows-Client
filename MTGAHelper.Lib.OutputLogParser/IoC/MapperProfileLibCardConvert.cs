using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogProgress;
using MTGAHelper.Web.UI.Shared;

namespace MTGAHelper.Lib.Ioc
{
    public class MapperProfileLibCardConvert : Profile
    {
        public MapperProfileLibCardConvert(
            AutoMapperEventNameToTypeConverter eventNameToType,
            DeckListConverter deckListConverter)
        {
            CreateMap<GameProgress, GameDetail>()
                .ForMember(i => i.OpponentCardsSeen, i => i.MapFrom(x => x.OpponentCardsSeenByInstanceId.Values
                    .GroupBy(grpId => grpId)
                    .ToDictionary(y => y.Key, y => y.Count())))
                .ForMember(i => i.CardTransfers, i => i.MapFrom(x => x.CardTransfersByTurn.SelectMany(y => y.Value).ToArray()));

            // To map all the opponent fields
            CreateMap<MatchCreatedResult, MatchResult>()
                .ForMember(i => i.StartDateTime, i => i.MapFrom(x => x.LogDateTime))
                .ForMember(i => i.EventName, i => i.MapFrom(x => x.Raw.payload.eventId))
                .ForMember(i => i.EventType, i => i.ConvertUsing(eventNameToType, x => x.Raw.payload.eventId))
                .ForMember(i => i.Opponent, i => i.MapFrom(x => x.Raw.payload))
                .ForMember(m => m.Games, o => o.Ignore())
                .ForMember(m => m.DeckUsed, o => o.Ignore());

            //CreateMap<CourseDeckRaw, MtgaDeck>()
            CreateMap<CourseDeckRaw, ConfigModelRawDeck>()
                .ForMember(i => i.DeckTileId, i => i.MapFrom(x => x.deckTileId ?? default(int)))
                .ForMember(i => i.CardCommander, i => i.MapFrom(x => x.commandZoneGRPId))
                .ForMember(i => i.CardsMain, i => i.ConvertUsing(deckListConverter, x => x.mainDeck))
                .ForMember(i => i.CardsSideboard, i => i.ConvertUsing(deckListConverter, x => x.sideboard))
                .ForMember(m => m.ArchetypeId, o => o.Ignore()) // todo Bruno?
                .ForMember(m => m.CardsMainWithCommander, o => o.Ignore());

            ////CreateMap<GetDeckListResultDeckRaw, MtgaDeck>()
            //CreateMap<GetDeckListResultDeckRaw, ConfigModelRawDeck>()
            //    .ForMember(i => i.CardsMain, i => i.MapFrom(x => deckListConverter.ConvertSimple(x.mainDeck)))
            //    .ForMember(i => i.CardsSideboard, i => i.MapFrom(x => deckListConverter.ConvertSimple(x.sideboard)))
            //    .ForMember(i => i.DeckTileId, i => i.MapFrom(x => x.deckTileId ?? default(int)));
        }
    }
}
