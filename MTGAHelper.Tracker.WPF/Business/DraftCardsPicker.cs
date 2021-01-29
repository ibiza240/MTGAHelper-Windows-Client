using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Web.Models.Response.User;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftCardsPicker
    {
        private readonly DraftPicksCalculator DraftPicksCalculator;
        private readonly IMapper mapper;
        private readonly CacheSingleton<Dictionary<string, DraftRatings>> draftRatings;

        public ICollection<CardDraftPickWpf> AllRatings { get; private set; }

        public DraftCardsPicker(
            DraftPicksCalculator draftPicksCalculator,
            IMapper mapper,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings
            )
        {
            DraftPicksCalculator = draftPicksCalculator;
            this.mapper = mapper;
            this.draftRatings = draftRatings;
        }

        public ICollection<CardDraftPickWpf> GetDraftPicksForCards(
            ICollection<int> cardPool,
            ICollection<int> pickedCards,
            string source,
            Dictionary<int, int> collection,
            ICollection<CardCompareInfo> raredraftingInfo,
            Dictionary<string, Dictionary<string, CustomDraftRating>> customRatingsBySetThenCardName
            )
        {
            //var apiResponse = api.GetCardsForDraftPick(userId, grpIds, source);

            AllRatings = draftRatings.Get().SelectMany(source => source.Value.RatingsBySet.SelectMany(set => set.Value.Ratings.Select(r => new CardDraftPickWpf
            {
                Name = r.CardName,
                RatingSource = source.Key,
                RatingValue = r.RatingValue,
                Set = set.Key,
                Description = r.Description,
            }))
            ).ToArray();

            var result = DraftPicksCalculator.Init(customRatingsBySetThenCardName).GetCardsForDraftPick(
                cardPool,
                pickedCards,
                source,
                collection,
                raredraftingInfo);

            var apiDto = mapper.Map<ICollection<CardForDraftPickDto>>(result);

            var ret = mapper.Map<ICollection<CardDraftPickWpf>>(apiDto);

            foreach (CardDraftPickWpf c in ret)
            {
                c.ImageArtUrl = Utilities.GetThumbnailLocal(c.ImageArtUrl);

                if (customRatingsBySetThenCardName.ContainsKey(c.Set) && customRatingsBySetThenCardName[c.Set].ContainsKey(c.Name))
                {
                    c.CustomRatingValue = customRatingsBySetThenCardName[c.Set][c.Name].Rating;
                    c.CustomRatingDescription = customRatingsBySetThenCardName[c.Set][c.Name].Note;
                }
            }

            return ret;
        }
    }
}
