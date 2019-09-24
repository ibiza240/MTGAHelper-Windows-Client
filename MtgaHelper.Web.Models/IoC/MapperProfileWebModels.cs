
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaDeckStats;
using MTGAHelper.Lib.Config.Users;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.Models;
using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.Models.SharedDto;
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
            ICollection<Card> allCards,
            IServiceProvider provider,
            UtilManaCurve utilManaCurve)
        {
            var rawDeckConverter = provider.GetService<AutoMapperRawDeckConverter>();
            var rawDeckToColorConverter = provider.GetService<AutoMapperRawDeckToColorConverter>();
            var utilColors = provider.GetService<UtilColors>();

            var dictAllCards = allCards.ToDictionary(i => i.grpId, i => i);

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
                .ForMember(i => i.CraftedOnly, i => i.MapFrom(x => x.Card.craftedOnly));


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
                .ForMember(i => i.FirstTurn, i => i.MapFrom(x => x.Games.First().FirstTurn.ToString()))
                .ForMember(i => i.OpponentRank, i => i.MapFrom(x => $"{x.Opponent.RankingClass}_{x.Opponent.RankingTier}"))
                //.ForMember(i => i.Outcome, i => i.MapFrom(x => string.Join('-', x.Games.Select(y => y.Outcome.ToString()[0]))))
                .ForMember(i => i.OpponentDeckColors, i => i.MapFrom(x => utilColors.FromGrpIds(x.GetOpponentCardsSeen())));

            // ConfigModelRawDeck to DeckDto directly
            CreateMap<ConfigModelRawDeck, SimpleDeckDto>()
                .ForMember(i => i.Main, i => i.MapFrom(x => x.CardsMain))
                .ForMember(i => i.Sideboard, i => i.MapFrom(x => x.CardsSideboard))
                .ForMember(i => i.Colors, i => i.ConvertUsing(rawDeckToColorConverter, x => x))
                .ForMember(i => i.DeckImage, i => i.MapFrom(x => dictAllCards[x.DeckTileId].imageArtUrl));

            CreateMap<GameDetail, GameDetailDto>();

            CreateMap<CardTurnAction, CardTurnActionDto>()
                .ForMember(i => i.Card, i => i.MapFrom(x => x.CardGrpId));

            CreateMap<Dictionary<int, int>, ICollection<CardWithAmountDto>>().ConvertUsing(rawDeckConverter);

            CreateMap<ConfigModelRankInfo, RankInfoDto>();

            CreateMap<HistorySummaryForDate, GetUserHistorySummaryDto>()
                .ForMember(i => i.Wins, i => i.MapFrom(x => x.OutcomesByMode.Values.Sum(y => y.Wins)))
                .ForMember(i => i.Losses, i => i.MapFrom(x => x.OutcomesByMode.Values.Sum(y => y.Losses)));

            CreateMap<DateSnapshotInfo, GetUserHistoryForDateResponseInfo>()
                .ForMember(i => i.ConstructedRank, i => i.MapFrom(x => x.RankInfo.FirstOrDefault(y => y.Format == ConfigModelRankInfoFormatEnum.Constructed) ?? new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Constructed)))
                .ForMember(i => i.LimitedRank, i => i.MapFrom(x => x.RankInfo.FirstOrDefault(y => y.Format == ConfigModelRankInfoFormatEnum.Limited) ?? new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Limited)))
                .ForMember(i => i.Gold, i => i.MapFrom(x => x.Inventory.Gold))
                .ForMember(i => i.Gems, i => i.MapFrom(x => x.Inventory.Gems))
                .ForMember(i => i.Wildcards, i => i.MapFrom(x => x.Inventory.Wildcards))
                .ForMember(i => i.VaultProgress, i => i.MapFrom(x => x.Inventory.VaultProgress));

            CreateMap<DateSnapshotDiff, GetUserHistoryForDateResponseDiff>();

            CreateMap<CardForDraftPick, CardForDraftPickDto>()
                .ForMember(i => i.IdArena, i => i.MapFrom(x => x.grpId));

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
        }
    }
}
