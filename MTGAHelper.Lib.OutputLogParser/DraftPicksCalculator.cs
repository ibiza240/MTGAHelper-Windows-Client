using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class DraftPicksCalculator
    {
        readonly Dictionary<int, Card> allCards;
        //CacheSingleton<Dictionary<string, DraftRatings>> draftRatings;
        readonly Dictionary<string,
            Dictionary<string,
                (Dictionary<string, DraftRating> ratings, Dictionary<string, ICollection<DraftRatingTopCard>> topCardsByColor)>> ratingsBySourceSet;

        public DraftPicksCalculator(
            CacheSingleton<Dictionary<int, Card>> cacheAllCards,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings)
        {
            this.allCards = cacheAllCards.Get();
            //this.draftRatings = draftRatings;
            var ratingsBySource = draftRatings.Get();
            ratingsBySourceSet = ratingsBySource.ToDictionary(
                source => source.Key, source => source.Value.RatingsBySet.ToDictionary(
                    set => set.Key, set => (
                        set.Value.Ratings.ToDictionary(card => card.CardName, card => card),
                        set.Value.TopCommonCardsByColor
                    )));
        }

        public ICollection<CardForDraftPick> GetCardsForDraftPick(
            string userId,
            string source,
            ICollection<int> grpIds,
            Dictionary<int, int> collection,
            ICollection<CardCompareInfo> raredraftingInfo
            )
        {
            if (grpIds.Any() == false)
                return new CardForDraftPick[0];

            var weights = raredraftingInfo.ToDictionary(i => i.GrpId, i => i);

            //var test = grpIds
            //    .Where(i => dictCardsByGrpId.ContainsKey(i))
            //    .Select(i => dictCardsByGrpId[i])
            //    .ToList();

            //var collection = ConfigUsers.Get(userId).DataInMemory.HistoryDetails.GetLastCollectionInMemory(rawDeckConverter).Info.ToDictionary(i => i.Card.grpId, i => i.Amount);
            //var collection = cacheUserHistoryCollectionIntraday.GetLast(userId).Info.Values.Last();

            var byCard = grpIds
                .Where(i => allCards.ContainsKey(i))
                .Select(i => allCards[i]);

            var maxWeight = byCard.Max(i => weights.ContainsKey(i.grpId) ? weights[i.grpId].MissingWeight : 0f);

            var dataUnordered = byCard
                //.Select(i => new { ratings = draftRatings.Get()[source], item = i })
                .Select(i =>
                {
                    //var i = ri.item;
                    //var draftRatings = ri.ratings;

                    var card = Mapper.Map<CardForDraftPick>(i);

                    try
                    {
                        card.NbMissingCollection = collection.ContainsKey(i.grpId) ? 4 - collection[i.grpId] : 4;
                        //var test = weights.FirstOrDefault(x => x.Value.Card.name.StartsWith("Find"));

                        if (weights.ContainsKey(i.grpId))
                        {
                            card.NbMissingTrackedDecks = weights[i.grpId].NbMissing;
                            card.Weight = weights[i.grpId].MissingWeight;
                            card.NbDecksUsedMain = weights[i.grpId].NbDecksMain;//.ByDeck.Count(x => x.Value.NbMissingMain > 0);
                            card.NbDecksUsedSideboard = weights[i.grpId].NbDecksSideboardOnly;//.NbDecks - card.NbDecksUsedMain;
                        }

                        //if (draftRatings.DictRatingByCardName.ContainsKey(i.name))
                        //{
                        //    card.Description = draftRatings.DictRatingByCardName[i.name].Description;
                        //    card.Rating = draftRatings.DictRatingByCardName[i.name].Rating;
                        //}

                        //if (draftRatings.RatingsBySet.ContainsKey(card.set) && draftRatings.RatingsBySet[card.set].TopCommonCardsByColor.ContainsKey(cardColors))
                        //{
                        //    var rank = draftRatings.RatingsBySet[card.set].TopCommonCardsByColor[card.colors.First()].FirstOrDefault(x => x.Name == card.name)?.Rank ?? 0;
                        //    card.TopCommonCard = new DraftRatingTopCard(rank, cardColors);
                        //}
                        if (ratingsBySourceSet.ContainsKey(source) &&
                            ratingsBySourceSet[source].ContainsKey(i.set) &&
                            ratingsBySourceSet[source][i.set].ratings.ContainsKey(i.name))
                        {
                            var draftRating = ratingsBySourceSet[source][i.set].ratings[i.name];
                            card.Description = draftRating.Description;
                            card.RatingValue = draftRating.RatingValue;
                            card.RatingToDisplay = draftRating.RatingToDisplay;
                            card.RatingSource = source;

                            var cardColors = string.Join("", card.colors);
                            if (ratingsBySourceSet[source][i.set].topCardsByColor.ContainsKey(cardColors))
                            {
                                var rank = ratingsBySourceSet[source][i.set].topCardsByColor[card.colors.First()].FirstOrDefault(x => x.Name == card.name)?.Rank ?? 0;
                                card.TopCommonCard = new DraftRatingTopCard(rank, cardColors);
                            }
                        }
                        else
                        {
                            // Rating not found for card
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "ERROR IN DRAFTPICK");
                        System.Diagnostics.Debugger.Break();
                    }

                    return card;
                });

            var data = dataUnordered
                .OrderByDescending(i => i.RatingValue)
                .ThenByDescending(i => i.Weight)
                .ThenByDescending(i => i.name)
                .ToArray();

            //DetermineRareDraft(data);
            var isMissingRareLand = data.FirstOrDefault(i => i.type.Contains("Land") && i.GetRarityEnum() == RarityEnum.Rare && i.NbMissingCollection > 0);

            if (isMissingRareLand != null)
                isMissingRareLand.IsRareDraftPick = RaredraftPickReasonEnum.RareLandMissing;
            else if (maxWeight > 0 && data.Any(i => i.Weight == maxWeight))
                data.First(i => i.Weight == maxWeight).IsRareDraftPick = RaredraftPickReasonEnum.HighestWeight;
            else if (data.Any(i => i.type.StartsWith("Basic Land") == false && i.NbMissingCollection > 0))
            {
                var r = data
                    .Where(i => i.type.StartsWith("Basic Land") == false)
                    .Where(i => i.NbMissingCollection > 0)
                    .OrderBy(i => i.GetRarityEnum())
                    .ThenByDescending(i => i.NbMissingCollection)
                    .First();
                r.IsRareDraftPick = RaredraftPickReasonEnum.MissingInCollection;
            }
            else
            {
                var bestRarity = data.OrderBy(i => i.GetRarityEnum()).First().GetRarityEnum();
                var cardsMatching = data
                    .Where(i => i.GetRarityEnum() == bestRarity)
                    .Where(i => i.type.StartsWith("Basic Land") == false);
                foreach (var c in cardsMatching)
                    c.IsRareDraftPick = RaredraftPickReasonEnum.BestVaultRarity;
            }

            return data;
        }
    }
}
