
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaDeckStats;
using MTGAHelper.Entity.UserHistory;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config.Users;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Web.Models;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.Models.Response.User.History;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response;
using MTGAHelper.Web.UI.Model.Response.Dto;
using MTGAHelper.Web.UI.Model.Response.User;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;
using MTGAHelper.Web.UI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.IoC
{
    public class MapperProfileWebModels : Profile
    {
        public MapperProfileWebModels(
            Dictionary<int, Card> dictAllCards,
            IServiceProvider provider,
            UtilManaCurve utilManaCurve)
        {
            var rawDeckConverter = provider.GetService<AutoMapperRawDeckConverter>();
            var rawDeckToColorConverter = provider.GetService<AutoMapperRawDeckToColorConverter>();
            var utilColors = provider.GetService<UtilColors>();
            var setsByCollationId = provider.GetService<CacheSingleton<Dictionary<int, Set>>>()?.Get();

            CreateMap<Card, CardDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.grpId));

            CreateMap<Card, CardDtoFull>();
            CreateMap<CardDtoFull, Card>();

            CreateMap<CardWithAmount, CardWithAmountDto>()
                .ForMember(i => i.Name, i => i.MapFrom(x => x.Card.name))
                .ForMember(i => i.Set, i => i.MapFrom(x => x.Card.set))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.GetRarityEnum(false).ToString()))
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => x.Card.imageCardUrl))
                //.ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => x.Card.imageArtUrl))
                .ForMember(i => i.Color, i => i.MapFrom(x => x.Card.type.Contains("Land") ? "Land" : x.Card.colors == null ? "" : string.Join("", x.Card.colors)))
            ;

            CreateMap<CardWithAmountDto, CollectionCardDto>();

            CreateMap<CardWithAmount, CollectionCardDto>()
                .IncludeBase<CardWithAmount, CardWithAmountDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.Card.grpId))
                .ForMember(i => i.NotInBooster, i => i.MapFrom(x => x.Card.notInBooster));


            //CreateMap<DateSnapshotInfo, GetUserHistoryDto>();
            //CreateMap<DateSnapshotDiff, GetUserHistoryDto>();
            ////CreateMap<UserHistorySnapshot, GetUserHistoryDto>()
            ////    .ForMember(i => i.GemsChange, i => i.MapFrom(x => x.Diff.GemsChange))
            ////    .ForMember(i => i.GoldChange, i => i.MapFrom(x => x.Diff.GoldChange))
            ////    .ForMember(i => i.NewCards, i => i.MapFrom(x => x.Diff.NewCards))
            ////    .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Diff.VaultProgress))
            ////    .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Diff.WildcardsChange));

            CreateMap<DeckCard, DeckCardDto>()
                .ForMember(i => i.Name, i => i.MapFrom(x => x.Card.name))
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => x.Card.imageCardUrl))
                .ForMember(i => i.ImageThumbnail, i => i.MapFrom(x => new Util().GetThumbnailUrl(x.Card.imageArtUrl)))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.GetRarityEnum(false).ToString()))
                .ForMember(i => i.Type, i => i.MapFrom(x => x.Card.GetSimpleType()))
                .ForMember(i => i.Set, i => i.MapFrom(x => x.Card.set))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.Card.mana_cost))
                .ForMember(i => i.Cmc, i => i.MapFrom(x => x.Card.cmc))
                .ForMember(i => i.Color, i => i.MapFrom(x => x.Card.type.Contains("Land") ? "Land" : x.Card.colors == null ? "" : string.Join("", x.Card.colors)));

            CreateMap<CardWithAmount, DeckCardDto>()
                .ForMember(i => i.Name, i => i.MapFrom(x => x.Card.name))
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => x.Card.imageCardUrl))
                .ForMember(i => i.ImageThumbnail, i => i.MapFrom(x => new Util().GetThumbnailUrl(x.Card.imageArtUrl)))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.GetRarityEnum(false).ToString()))
                .ForMember(i => i.Type, i => i.MapFrom(x => x.Card.GetSimpleType()))
                .ForMember(i => i.Set, i => i.MapFrom(x => x.Card.set))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.Card.mana_cost))
                .ForMember(i => i.Cmc, i => i.MapFrom(x => x.Card.cmc))
                .ForMember(i => i.Color, i => i.MapFrom(x => x.Card.type.Contains("Land") ? "Land" : x.Card.colors == null ? "" : string.Join("", x.Card.colors)));

            //new DeckCardDto
            //{
            //    Name = i.Card.name,
            //    ImageCardUrl = i.Card.imageCardUrl,//.images["normal"],
            //                                       //ImageArtUrl = i.Card.imageArtUrl,//.images["normal"],
            //    Rarity = i.Card.GetRarityEnum(false).ToString(),
            //    Type = i.Card.GetSimpleType(),
            //    Amount = i.Amount,
            //    NbMissing =
            //        hCards[i.IsSideboard].Contains(i.Card.name) ? 0 : (deckInfo.CardsRequired.ByCard.ContainsKey(i.Card) ?
            //        (i.IsSideboard ? deckInfo.CardsRequired.ByCard[i.Card].NbMissingSideboard : deckInfo.CardsRequired.ByCard[i.Card].NbMissingMain) : 0),
            //}

            CreateMap<MatchResult, MatchDto>()
                .IncludeBase<MatchResult, MatchDtoLightweight>();

            CreateMap<MatchResult, MatchDtoLightweight>()
                .ForMember(i => i.OpponentName, i => i.MapFrom(x => x.Opponent.ScreenName))
                .ForMember(i => i.FirstTurn, i => i.MapFrom(x => x.Games.Any() == false ? FirstTurnEnum.Unknown.ToString() : x.Games.First().FirstTurn.ToString()))
                .ForMember(i => i.OpponentRank, i => i.MapFrom(x => $"{x.Opponent.RankingClass}_{x.Opponent.RankingTier}"))
                //.ForMember(i => i.Outcome, i => i.MapFrom(x => string.Join('-', x.Games.Select(y => y.Outcome.ToString()[0]))))
                .ForMember(i => i.OpponentDeckColors, i => i.MapFrom(x => utilColors.FromGrpIds(x.GetOpponentCardsSeen())));

            // ConfigModelRawDeck to DeckDto directly
            CreateMap<ConfigModelRawDeck, SimpleDeckDto>()
                .ForMember(i => i.Main, i => i.MapFrom(x => x.CardsMain))
                .ForMember(i => i.Sideboard, i => i.MapFrom(x => x.CardsSideboard))
                .ForMember(i => i.Colors, i => i.ConvertUsing(rawDeckToColorConverter, x => x))
                .ForMember(i => i.DeckImage, i => i.MapFrom(x => (string)dictAllCards[x.DeckTileId].imageArtUrl));

            CreateMap<GameDetail, GameDetailDto>();

            CreateMap<CardTurnAction, CardTurnActionDto>()
                .ForMember(i => i.Card, i => i.MapFrom(x => x.CardGrpId));

            CreateMap<Dictionary<int, int>, ICollection<CardWithAmountDto>>().ConvertUsing(rawDeckConverter);

            CreateMap<ConfigModelRankInfo, RankInfoDto>();

            CreateMap<HistorySummaryForDate, GetUserHistorySummaryDto>()
                .ForMember(i => i.Wins, i => i.MapFrom(x => x.OutcomesByMode.Values.Sum(y => y.Wins)))
                .ForMember(i => i.Losses, i => i.MapFrom(x => x.OutcomesByMode.Values.Sum(y => y.Losses)))
                .ForMember(i => i.BoostersChange, i => i.MapFrom(x => x.BoostersChange.Select(b => new KeyValuePair<string, int>(b.Key, b.Value))));

            //CreateMap<DateSnapshotInfo, GetUserHistoryForDateResponseInfo>()
            //    .ForMember(i => i.ConstructedRank, i => i.MapFrom(x => x.RankSynthetic.FirstOrDefault(y => y.Format == RankFormatEnum.Constructed) ?? new ConfigModelRankInfo(RankFormatEnum.Constructed)))
            //    .ForMember(i => i.LimitedRank, i => i.MapFrom(x => x.RankSynthetic.FirstOrDefault(y => y.Format == RankFormatEnum.Limited) ?? new ConfigModelRankInfo(RankFormatEnum.Limited)))
            //    .ForMember(i => i.Gold, i => i.MapFrom(x => x.Inventory.Gold))
            //    .ForMember(i => i.Gems, i => i.MapFrom(x => x.Inventory.Gems))
            //    .ForMember(i => i.Wildcards, i => i.MapFrom(x => x.Inventory.Wildcards))
            //    .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Inventory.VaultProgress));

            //CreateMap<DateSnapshotDiff, GetUserHistoryForDateResponseDiff>();

            CreateMap<CardForDraftPick, CardForDraftPickDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.grpId))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.mana_cost));

            CreateMap<int, CardDto>()
                .ConvertUsing(i => Mapper.Map<CardDto>(Mapper.Map<Card>(i)));

            CreateMap<MtgaDeckSummary, MtgaDeckSummaryDto>()
                .ForMember(i => i.FirstPlayed, i => i.MapFrom(x => x.FirstPlayed.ToString("yyyy-MM-dd")))
                .ForMember(i => i.LastPlayed, i => i.MapFrom(x => x.LastPlayed.ToString("yyyy-MM-dd")));
            //.ForMember(i => i.DeckColor, i => i.ConvertUsing(rawDeckToColorConverter, x => x.DeckUsed));

            CreateMap<MtgaDeckDetail, MtgaDeckDetailDto>()
            //.ForMember(i => i.FirstPlayed, i => i.MapFrom(x => x.FirstPlayed.ToString("yyyy-MM-dd")))
            //.ForMember(i => i.LastPlayed, i => i.MapFrom(x => x.LastPlayed.ToString("yyyy-MM-dd")));
            //.ForMember(i => i.DeckColor, i => i.ConvertUsing(rawDeckToColorConverter, x => x.DeckUsed))
            //.ForMember(i => i.CardsMain, i => i.MapFrom(x => x.DeckUsed.CardsMain))
            //.ForMember(i => i.CardsSideboard, i => i.MapFrom(x => x.DeckUsed.CardsSideboard));
                .ForMember(i => i.CardsMain, i => i.MapFrom(x => Mapper.Map<ICollection<CardWithAmount>>(x.CardsMain)))
                .ForMember(i => i.CardsSideboard, i => i.MapFrom(x => Mapper.Map<ICollection<CardWithAmount>>(x.CardsSideboard)))
                .ForMember(i => i.ManaCurve, i => i.MapFrom(x => utilManaCurve.CalculateManaCurve(Mapper.Map<ICollection<CardWithAmount>>(x.CardsMain))));

            CreateMap<MtgaDeckAnalysis, MtgaDeckAnalysisDto>();
            CreateMap<MtgaDeckAnalysisMatchInfo, MtgaDeckAnalysisMatchInfoDto>();

            CreateMap<PlayerProgress, PlayerProgressDto>();

            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(i => i.Boosters, i => i.MapFrom(x => x.Boosters));

            CreateMap<InventoryBooster, InventoryBoosterDto>()
                .ForMember(i => i.Set, i => i.MapFrom(x => setsByCollationId[x.CollationId].Code));

            CreateMap<DeckSummary, DeckSummaryResponseDto>();
            CreateMap<DeckTrackedSummary, DeckTrackedSummaryResponseDto>();
                //.ForMember(i => i.MissingWeightBase, i => i.MapFrom(x => x.MissingWeightBase == float.NaN ? 0 : x.MissingWeightBase))
                //.ForMember(i => i.MissingWeight, i => i.MapFrom(x => x.MissingWeight == float.NaN ? 0 : x.MissingWeight))
                //.ForMember(i => i.PriorityFactor, i => i.MapFrom(x => x.PriorityFactor == float.NaN ? 0 : x.PriorityFactor));

            CreateMap<CardMissingDetailsModel, CardMissingDetailsModelResponseDto>();
            CreateMap<InfoCardMissingSummary, InfoCardMissingSummaryResponseDto>();

            CreateMap<AccountModel, AccountResponse>();

            CreateMap<EconomyEvent, EconomyEventDto>()
                .ForMember(i => i.Context, i => i.MapFrom(x => x.Context.Replace("PlayerInventory.", "")));

            CreateMap<EconomyEventChange, EconomyEventChangeDto>();

            CreateMap<Rank, RankDto>();
            CreateMap<RankDelta, RankDeltaDto>();
        }
    }
}
