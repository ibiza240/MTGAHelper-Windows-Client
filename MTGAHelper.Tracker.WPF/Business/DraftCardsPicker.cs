using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Web.Models.Response.User;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftCardsPicker
    {
        readonly DraftPicksCalculator DraftPicksCalculator;
        readonly IMapper mapper;

        public DraftCardsPicker(DraftPicksCalculator draftPicksCalculator, IMapper mapper)
        {
            DraftPicksCalculator = draftPicksCalculator;
            this.mapper = mapper;
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
