using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config;

namespace MTGAHelper.Entity.IoC
{
    public class MapperProfileOldCardFormat : Profile
    {
        public MapperProfileOldCardFormat()
        {
            CreateMap<Card2, Card>()
                .ForMember(i => i.type, i => i.MapFrom(x => x.Type_line))
                .ForMember(i => i.notInBooster, i => i.MapFrom(x => !x.IsInBooster))
                .ForMember(i => i.artistCredit, i => i.MapFrom(x => x.Artist));
        }
    }

    public class MapperProfileEntity : Profile
    {
        public MapperProfileEntity(CacheSingleton<Dictionary<int, Card>> cacheCards)
        {
            var dictAllCards = cacheCards.Get();

            // grpId to Card
            CreateMap<int, Card>().ConvertUsing(i => dictAllCards.ContainsKey(i) ? dictAllCards[i] :
                new Card
                {
                    grpId = 0,
                    name = "Unknown",
                    imageCardUrl = "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on"
                });

            //CreateMap<MtgaDeck, ConfigModelRawDeck>();

            CreateMap<KeyValuePair<int, int>, CardWithAmount>()
                .ConvertUsing(i => new CardWithAmount(dictAllCards[i.Key], i.Value));

            CreateMap<QuestUpdate, PlayerQuest>();
            CreateMap<TrackDiff, PlayerProgress>();
            CreateMap<DraftMakePickRaw, DraftPickProgress>();

            CreateMap<ConfigModelRawDeck, ConfigModelDeck>()
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
                .ForMember(i => i.Rating, i => i.Ignore())
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
