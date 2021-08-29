using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.CollectionDecksCompare;
using MTGAHelper.Entity.Config.Decks;
using MTGAHelper.Entity.GameEvents;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IoC
{
    public class MapperProfileEntity : Profile
    {
        public MapperProfileEntity(
            AutoMapperGrpIdToCardConverter grpIdToCard)
        {
            // grpId to Card
            CreateMap<int, Card>().ConvertUsing(grpIdToCard);

            //CreateMap<MtgaDeck, ConfigModelRawDeck>();

            CreateMap<KeyValuePair<int, int>, CardWithAmount>()
                .ForCtorParam("card", opt => opt.MapFrom(kvp => kvp.Key))
                .ForCtorParam("amount", opt => opt.MapFrom(kvp => kvp.Value))
                .ForAllMembers(o => o.Ignore());

            CreateMap<DeckCardRaw, CardWithAmount>()
                .ForMember(i => i.Card, i => i.MapFrom(x => x.GrpId));

            //CreateMap<QuestUpdate, PlayerQuest>();// TODO: delete?
            CreateMap<TrackDiff, PlayerProgress>(MemberList.None);
            CreateMap<DraftPickStatusRaw, DraftPickProgress>();

            CreateMap<ConfigModelRawDeck, ConfigModelDeck>(MemberList.None)
                .ForMember(i => i.ScraperTypeId, i => i.MapFrom(x => Constants.USERDECK_SOURCE_MTGADECK))
                .ForMember(i => i.UrlDeckList, i => i.MapFrom(x => (string)null));

            CreateMap<RankUpdatedRaw, Rank>()
                .ForMember(i => i.Format, i => i.MapFrom(x => Enum.Parse(typeof(RankFormatEnum), x.rankUpdateType)))
                .ForMember(i => i.Class, i => i.MapFrom(x => x.newClass))
                .ForMember(i => i.Level, i => i.MapFrom(x => x.newLevel))
                .ForMember(i => i.Step, i => i.MapFrom(x => x.newStep));

            CreateMap<CardRequiredInfoByCard, CardCompareInfo>()
                .ForMember(i => i.GrpId, i => i.MapFrom(x => x.Card.grpId))
                .ForMember(i => i.MissingWeight, i => i.MapFrom(x => x.MissingWeight))
                .ForMember(i => i.NbDecksMain, i => i.MapFrom(x => x.ByDeck.Count(y => y.Value.NbMissingMain > 0)))
                .ForMember(i => i.NbDecksSideboardOnly, i => i.MapFrom(x => x.NbDecks - x.ByDeck.Count(y => y.Value.NbMissingMain > 0)))
                .ForMember(i => i.NbMissing, i => i.MapFrom(x => x.NbMissing));

            CreateMap<IGameEvent, OutputLogResultGameEvent>()
                .ForMember(i => i.CardGrpId, i => i.MapFrom(x => x.Card.grpId));
        }
    }
}
