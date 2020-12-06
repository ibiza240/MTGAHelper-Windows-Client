using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class DraftPicksCalculator
    {
        readonly IMapper mapper;
        readonly Dictionary<int, Card> allCards;
        //CacheSingleton<Dictionary<string, DraftRatings>> draftRatings;
        readonly Dictionary<string,
            Dictionary<string,
                (Dictionary<string, DraftRating> ratings, Dictionary<string, ICollection<DraftRatingTopCard>> topCardsByColor)>> ratingsBySourceSet;

        public DraftPicksCalculator(
            IMapper mapper,
            CacheSingleton<Dictionary<int, Card>> cacheAllCards,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings)
        {
            this.mapper = mapper;
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

        public DraftPicksCalculator Init(Dictionary<string, Dictionary<string, CustomDraftRating>> customRatingsBySetThenCardName)
        {
            var dictCustomRatings = customRatingsBySetThenCardName
                .ToDictionary(i => i.Key, i => (
                    i.Value.ToDictionary(x => x.Key, x => new DraftRating
                    {
                        CardName = x.Value.Name,
                        Description = x.Value.Note,
                        RatingValue = x.Value.Rating ?? 0f,
                        RatingToDisplay = x.Value.Rating?.ToString() ?? "N/A",
                    }),
                    (Dictionary<string, ICollection<DraftRatingTopCard>>)null
                ));

            ratingsBySourceSet["Your custom ratings"] = dictCustomRatings;

            return this;
        }

        public ICollection<CardForDraftPick> GetCardsForDraftPick(
            ICollection<int> cardPool,
            ICollection<int> pickedCards,
            string source,
            Dictionary<int, int> collection,
            ICollection<CardCompareInfo> raredraftingInfo
            )
        {
            if (cardPool.Any() == false)
                return new CardForDraftPick[0];

            var weights = raredraftingInfo.ToDictionary(i => i.GrpId, i => i);

            var byCard = cardPool
                .Where(i => allCards.ContainsKey(i))
                .Select(i => allCards[i]);

            var maxWeight = byCard.Max(i => weights.ContainsKey(i.grpId) ? weights[i.grpId].MissingWeight : 0f);

            var dataUnordered = byCard
                .Select(i =>
                {
                    var card = new CardForDraftPick { Card = i };

                    try
                    {
                        if (source == null)
                            Log.Error("GetCardsForDraftPick Source is null");
                        if (i.set == null)
                            Log.Error("GetCardsForDraftPick Card set is null {grpId}", i.grpId);
                        if (i.name == null)
                            Log.Error("GetCardsForDraftPick Card name is null", i.grpId);

                        card.NbMissingCollection = calculateMissingAmount(collection, i, pickedCards);

                        if (weights.ContainsKey(i.grpId))
                        {
                            card.NbMissingTrackedDecks = weights[i.grpId].NbMissing;
                            card.Weight = weights[i.grpId].MissingWeight;
                            card.NbDecksUsedMain = weights[i.grpId].NbDecksMain;
                            card.NbDecksUsedSideboard = weights[i.grpId].NbDecksSideboardOnly;
                        }

                        Dictionary<string, DraftRating> ratings;
                        Dictionary<string, ICollection<DraftRatingTopCard>> topCardsByColor;
                        if (ratingsBySourceSet.ContainsKey(source) &&
                            ratingsBySourceSet[source].ContainsKey(i.set))
                        {
                            (ratings, topCardsByColor) = ratingsBySourceSet[source][i.set];
                        }
                        else
                        {
                            // This source does not have ratings for this set; use the first source that does
                            var sourceWithSet = ratingsBySourceSet.FirstOrDefault(s => s.Value.ContainsKey(i.set));
                            (ratings, topCardsByColor) = sourceWithSet.Value[i.set];
                            source = sourceWithSet.Key;
                        }

                        if (!ratings.ContainsKey(i.name))
                            // Rating not found for card
                            return card;

                        var draftRating = ratings[i.name];
                        card.Description = draftRating.Description;
                        card.RatingValue = draftRating.RatingValue;
                        card.RatingToDisplay = draftRating.RatingToDisplay;
                        card.RatingSource = source;

                        var cardColors = string.Join("", i.colors);
                        if (topCardsByColor?.ContainsKey(cardColors) == true)
                        {
                            var rank = topCardsByColor[i.colors.First()]
                                .FirstOrDefault(x => x.Name == i.name)?.Rank ?? 0;
                            card.TopCommonCard = new DraftRatingTopCard(rank, cardColors);
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
                .ThenByDescending(i => i.Card.name)
                .ToArray();

            var isMissingRareLand = data.FirstOrDefault(i => i.Card.type.Contains("Land") && i.Card.GetRarityEnum() == RarityEnum.Rare && i.NbMissingCollection > 0);

            if (isMissingRareLand != null)
                isMissingRareLand.IsRareDraftPick = RaredraftPickReasonEnum.RareLandMissing;

            else if (maxWeight > 0 && data.Any(i => i.Weight == maxWeight))
                data.First(i => i.Weight == maxWeight).IsRareDraftPick = RaredraftPickReasonEnum.HighestWeight;

            else if (data.Any(i => i.Card.type.StartsWith("Basic Land") == false && i.NbMissingCollection > 0))
            {
                var r = data
                    .Where(i => i.Card.type.StartsWith("Basic Land") == false)
                    .Where(i => i.NbMissingCollection > 0)
                    .OrderBy(i => i.Card.GetRarityEnum())
                    .ThenByDescending(i => i.NbMissingCollection)
                    .First();

                r.IsRareDraftPick = RaredraftPickReasonEnum.MissingInCollection;
            }
            else
            {
                var bestRarity = data.Select(c => c.Card.GetRarityEnum()).Min();
                var cardsMatching = data
                    .Where(i => i.Card.GetRarityEnum() == bestRarity)
                    .Where(i => i.Card.type.StartsWith("Basic Land") == false);

                foreach (var c in cardsMatching)
                    c.IsRareDraftPick = RaredraftPickReasonEnum.BestVaultRarity;
            }

            return data;
        }

        private int calculateMissingAmount(
            Dictionary<int, int> collection,
            Card card,
            ICollection<int> pickedCards)
        {
            // Check with collection
            var missingAmount = card.type.StartsWith("Basic Land") ? 0 :
                collection.ContainsKey(card.grpId) ? 4 - collection[card.grpId] : 4;

            // Consider picked cards during draft
            missingAmount -= pickedCards.Count(i => i == card.grpId);

            return Math.Max(0, missingAmount);
        }
    }
}
