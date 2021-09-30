using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.CollectionDecksCompare;
using MTGAHelper.Entity.Config.Users;
using MTGAHelper.Entity.MtgaDeckStats;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Entity.UserHistory;
using MTGAHelper.Lib;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.Models.Response.Dashboard;
using MTGAHelper.Web.Models.Response.Deck;
using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.Models.Response.User.History;
using MTGAHelper.Web.Models.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.IoC
{
    public class MapperProfileWebModels : Profile
    {
        public MapperProfileWebModels(
            UtilColors utilColors,
            AutoMapperRawDeckToColorConverter rawDeckToColor,
            AutoMapperIntToCardArtConverter intToCardArt,
            AutoMapperRawDeckConverter rawDeckConverter,
            AutoMapperManaCurveConverter manaCurveConverter,
            AutoMapperCollationToSetConverter collationToSet,
            AutoMapperDictCardsByZoneConverter dictCardsByZoneConverter)
        {
            CreateMap<Card, CardDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.grpId))
                .ForMember(i => i.Name, i => i.MapFrom(x => x.name.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()));

            CreateMap<CardWithAmount, CardWithAmountDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.Card.grpId))
                .ForMember(i => i.Name, i => i.MapFrom(x => x.Card.name))
                .ForMember(i => i.Set, i => i.MapFrom(x => x.Card.set))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.GetRarityEnum(false).ToString()))
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => x.Card.imageCardUrl))
                //.ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => x.Card.imageArtUrl))
                .ForMember(i => i.Color, i => i.MapFrom(x => x.Card.type.Contains("Land") ? "Land" : x.Card.colors == null ? "" : string.Join("", x.Card.colors)))
            ;

            //CreateMap<CardWithAmountDto, CollectionCardDto>();

            CreateMap<CardWithAmount, CollectionCardDto>()
                .IncludeBase<CardWithAmount, CardWithAmountDto>()
                .ForMember(i => i.NotInBooster, i => i.MapFrom(x => x.Card.notInBooster))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.rarity));

            //CreateMap<DateSnapshotInfo, GetUserHistoryDto>();
            //CreateMap<DateSnapshotDiff, GetUserHistoryDto>();
            ////CreateMap<UserHistorySnapshot, GetUserHistoryDto>()
            ////    .ForMember(i => i.GemsChange, i => i.MapFrom(x => x.Diff.GemsChange))
            ////    .ForMember(i => i.GoldChange, i => i.MapFrom(x => x.Diff.GoldChange))
            ////    .ForMember(i => i.NewCards, i => i.MapFrom(x => x.Diff.NewCards))
            ////    .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Diff.VaultProgress))
            ////    .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Diff.WildcardsChange));

            CreateMap<DeckCard, DeckCardDto>()
                .IncludeBase<CardWithAmount, DeckCardDto>()
                .ForMember(i => i.Zone, i => i.MapFrom(x => x.Zone.ToString()));

            CreateMap<DeckCardRaw, DeckCard>()
                .ForMember(i => i.Card, i => i.MapFrom(x => x.GrpId));

            CreateMap<CardWithAmount, DeckCardDto>()
                .ForMember(i => i.Name, i => i.MapFrom(x => x.Card.name))
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => x.Card.imageCardUrl))
                .ForMember(i => i.ImageThumbnail, i => i.MapFrom(x => new Util().GetThumbnailUrl(x.Card.imageArtUrl)))
                .ForMember(i => i.Rarity, i => i.MapFrom(x => x.Card.GetRarityEnum(false).ToString()))
                .ForMember(i => i.Type, i => i.MapFrom(x => x.Card.GetSimpleType()))
                .ForMember(i => i.Set, i => i.MapFrom(x => x.Card.set))
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.Card.grpId))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.Card.mana_cost))
                .ForMember(i => i.Cmc, i => i.MapFrom(x => x.Card.cmc))
                .ForMember(i => i.Color, i => i.MapFrom(x => x.Card.type.Contains("Land") ? "Land" : x.Card.colors == null ? "" : string.Join("", x.Card.colors)))
                .ForMember(m => m.NbMissing, o => o.Ignore())
                .ForMember(m => m.Zone, o => o.Ignore());

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
                .ForMember(i => i.OpponentRank, i => i.MapFrom(x => x.Opponent.GetRankString()))
                .ForMember(i => i.OpponentRankImg, i => i.MapFrom(x => $"{x.Opponent.RankingClass}_{x.Opponent.RankingTier}"))
                //.ForMember(i => i.Outcome, i => i.MapFrom(x => string.Join('-', x.Games.Select(y => y.Outcome.ToString()[0]))))
                .ForMember(i => i.OpponentDeckColors, i => i.ConvertUsing(utilColors, x => x.GetOpponentCardsSeen()))
                .ForMember(m => m.RankDelta, o => o.Ignore());

            // ConfigModelRawDeck to DeckDto directly
            CreateMap<ConfigModelRawDeck, SimpleDeckDto>()
                .ForMember(i => i.Cards, i => i.MapFrom((src, dst, dstMb, ctx) => ctx.Mapper.Map<ICollection<DeckCard>>(src.Cards)))
                .ForMember(i => i.Colors, i => i.ConvertUsing(rawDeckToColor, x => x))
                .ForMember(i => i.DeckImage, i => i.ConvertUsing(intToCardArt, x => x.DeckTileId));

            CreateMap<GameDetail, GameDetailDto>();

            CreateMap<CardTurnAction, CardTurnActionDto>()
                .ForMember(i => i.Card, i => i.MapFrom(x => x.CardGrpId));

            CreateMap<Dictionary<int, int>, ICollection<CardWithAmountDto>>().ConvertUsing(rawDeckConverter);
            CreateMap<IReadOnlyDictionary<int, int>, ICollection<CardWithAmountDto>>().ConvertUsing(rawDeckConverter);

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

            CreateMap<Card, CardForDraftPickDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.grpId))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.mana_cost))
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

            CreateMap<CardForDraftPick, CardForDraftPickDto>()
                .IncludeMembers(c => c.Card);

            CreateMap<int, CardDto>()
                .ConvertUsing((i, dto, ctx) => ctx.Mapper.Map<CardDto>(ctx.Mapper.Map<Card>(i)));

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
                .ForMember(i => i.CardsMain, i => i.MapFrom((src, dst, dstMb, ctx) => ctx.Mapper.Map<ICollection<CardWithAmount>>(src.CardsMain)))
                .ForMember(i => i.CardsNotMainByZone, i => i.MapFrom((src, dst, dstMb, ctx) => ctx.Mapper.Map<ICollection<KeyValuePair<string, DeckCardDto[]>>>(src.CardsNotMainByZone)))
                .ForMember(i => i.ManaCurve, i => i.ConvertUsing(manaCurveConverter, x => x.CardsMain));

            CreateMap<MtgaDeckAnalysis, MtgaDeckAnalysisDto>();
            CreateMap<MtgaDeckAnalysisMatchInfo, MtgaDeckAnalysisMatchInfoDto>();

            CreateMap<PlayerProgress, PlayerProgressDto>();

            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(i => i.Boosters, i => i.MapFrom(x => x.Boosters));

            CreateMap<InventoryBooster, InventoryBoosterDto>()
                .ForMember(i => i.Set, i => i.ConvertUsing(collationToSet, x => x.CollationId));

            CreateMap<DeckSummary, DeckSummaryResponseDto>();
            CreateMap<DeckTrackedSummary, DeckTrackedSummaryResponseDto>();

            CreateMap<CardMissingDetailsModel, CardMissingDetailsModelResponseDto>();
            CreateMap<InfoCardMissingSummary, InfoCardMissingSummaryResponseDto>();
            CreateMap<DashboardModelSummary, DashboardModelSummaryDto>();

            CreateMap<AccountModel, AccountResponse>()
                .ForMember(m => m.IsAuthenticated, o => o.Ignore())
                .ForMember(m => m.ResponseStatus, o => o.Ignore())
                .ForMember(m => m.Message, o => o.Ignore());

            CreateMap<EconomyEvent, EconomyEventDto>()
                .ForMember(i => i.Context, i => i.MapFrom(x => x.Context.Replace("PlayerInventory.", "")));

            CreateMap<EconomyEventChange, EconomyEventChangeDto>();

            CreateMap<Rank, RankDto>();
            CreateMap<RankDelta, RankDeltaDto>();

            CreateMap<Dictionary<DeckCardZoneEnum, ICollection<DeckCardRaw>>, ICollection<KeyValuePair<string, DeckCardDto[]>>>()
                .ConvertUsing(dictCardsByZoneConverter);
        }
    }
}