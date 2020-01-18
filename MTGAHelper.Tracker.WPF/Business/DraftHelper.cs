using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Web.Models.Response.User;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftHelper
    {
        readonly DraftPicksCalculator draftPicksCalculator;

        public DraftHelper(
            DraftPicksCalculator draftPicksCalculator
            )
        {
            this.draftPicksCalculator = draftPicksCalculator;
        }

        public ICollection<CardDraftPickWpf> GetDraftPicksForCards(string userId, ICollection<int> grpIds, string source, Dictionary<int, int> collection, ICollection<CardCompareInfo> raredraftingInfo)
        {
            //var apiResponse = api.GetCardsForDraftPick(userId, grpIds, source);


            var result = draftPicksCalculator.GetCardsForDraftPick(userId, source, grpIds, collection, raredraftingInfo);
            var apiDto = Mapper.Map<ICollection<CardForDraftPickDto>>(result);

            var ret = Mapper.Map<ICollection<CardDraftPickWpf>>(apiDto);
            foreach (var r in ret)
                r.DraftRatingSource = source;

            return ret;
        }
    }
}
