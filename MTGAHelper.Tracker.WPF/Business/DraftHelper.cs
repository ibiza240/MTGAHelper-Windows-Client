using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Models.OutputLog;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
using Newtonsoft.Json;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftHelper
    {
        DraftPicksCalculator draftPicksCalculator;

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
            return Mapper.Map<ICollection<CardDraftPickWpf>>(apiDto);
        }
    }
}
