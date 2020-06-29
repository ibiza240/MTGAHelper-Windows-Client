using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.CollectionDecksCompare;
using MTGAHelper.Entity.Config.Decks;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IoC
{
    public class MapperProfileOldCardFormat : Profile
    {
        public MapperProfileOldCardFormat()
        {
            CreateMap<Card2, Card>()
                .ForMember(i => i.type, i => i.MapFrom(x => x.TypeLine))
                .ForMember(i => i.mana_cost, i => i.MapFrom(x => x.ManaCost))
                .ForMember(i => i.colors, i => i.MapFrom(x => x.Colors))
                .ForMember(i => i.color_identity, i => i.MapFrom(x => x.ColorIdentity))
                .ForMember(i => i.notInBooster, i => i.MapFrom(x => !x.IsInBooster))
                .ForMember(i => i.artistCredit, i => i.MapFrom(x => x.Artist))
                .ForMember(i => i.set, i => i.MapFrom(x => x.SetScryfall.ToUpper()));
        }
    }

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

            //CreateMap<QuestUpdate, PlayerQuest>();// TODO: delete?
            CreateMap<TrackDiff, PlayerProgress>(MemberList.None);
            CreateMap<DraftMakePickRaw, DraftPickProgress>();

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

            CreateMap<Card, CardForDraftPick>()
                .ForMember(i => i.RatingToDisplay, i => i.Ignore())
                .ForMember(i => i.RatingValue, i => i.Ignore())
                .ForMember(i => i.RatingSource, i => i.Ignore())
                .ForMember(i => i.Description, i => i.Ignore())
                .ForMember(i => i.Weight, i => i.Ignore())
                .ForMember(i => i.NbDecksUsedMain, i => i.Ignore())
                .ForMember(i => i.NbDecksUsedSideboard, i => i.Ignore())
                .ForMember(i => i.IsRareDraftPick, i => i.Ignore())
                .ForMember(i => i.NbMissingTrackedDecks, i => i.Ignore())
                .ForMember(i => i.NbMissingCollection, i => i.Ignore())
                .ForMember(i => i.TopCommonCard, i => i.Ignore());
        }
    }
}
