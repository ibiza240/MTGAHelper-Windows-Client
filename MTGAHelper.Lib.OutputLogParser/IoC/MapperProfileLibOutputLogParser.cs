using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.IoC
{
    public class MapperProfileLibOutputLogParser : Profile
    {
        public MapperProfileLibOutputLogParser()
        {
            //CreateMap<MatchCreatedRaw, MatchOpponentInfo>()
            //    .ForMember(i => i.IsWotc, i => i.MapFrom(x => x.opponentIsWotc))
            //    .ForMember(i => i.MythicLeaderboardPlace, i => i.MapFrom(x => x.opponentMythicLeaderboardPlace))
            //    .ForMember(i => i.MythicPercentile, i => i.MapFrom(x => x.opponentMythicPercentile))
            //    .ForMember(i => i.RankingClass, i => i.MapFrom(x => x.opponentRankingClass))
            //    .ForMember(i => i.RankingTier, i => i.MapFrom(x => x.opponentRankingTier))
            //    .ForMember(i => i.ScreenName, i => i.MapFrom(x => x.opponentScreenName));

            //CreateMap<GameDetail, ConfigModelUserMatchGameDetail>();
            //CreateMap<IDeck, ConfigModelRawDeck>()
            //    .ForMember(i => i.CardsMain, i => i.MapFrom(x => x.Cards.QuickCardsMain.ToDictionary(y => y.Key, y => y.Value.Amount)))
            //    .ForMember(i => i.CardsSideboard, i => i.MapFrom(x => x.Cards.QuickCardsSideboard.ToDictionary(y => y.Key, y => y.Value.Amount)));

            //CreateMap<GetPlayerCardsResult, Collection>();

            CreateMap<GetPlayerInventoryRaw, Inventory>()
                .ForMember(i => i.Wildcards, i => i.MapFrom(x => new Dictionary<RarityEnum, int>
                {
                    { RarityEnum.Common, x.wcCommon },
                    { RarityEnum.Uncommon, x.wcUncommon },
                    { RarityEnum.Rare, x.wcRare },
                    { RarityEnum.Mythic, x.wcMythic },
                }))
                .ForMember(m => m.Xp, o => o.Ignore()); // todo Bruno?

            CreateMap<BoosterDelta, InventoryBooster>();

            CreateMap<BattlePass, PlayerProgress>();
            CreateMap<GetPlayerQuestRaw, PlayerQuest>();

            CreateMap<Models.UnityCrossThreadLogger.DeckSummary, CourseDeckRaw>()
                .ForMember(i => i.id, i => i.MapFrom(x => x.DeckId))
                .ForMember(i => i.lastUpdated, i => i.MapFrom(x => x.Attributes.FirstOrDefault(i => i.Name == "lastPlayed").Value))
                //.ForMember(i => i.mainDeck, i => i.MapFrom(x => new List<int>()))
                ;
        }
    }
}