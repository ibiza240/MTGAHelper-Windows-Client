using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Models.OutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftHelper
    {
        ServerApiCaller api;

        Regex regexDraftMakePickModel = new Regex(@"<== Draft\.MakePick.*?({.*?})", RegexOptions.Compiled | RegexOptions.Singleline);

        public DraftHelper(ServerApiCaller api)
        {
            this.api = api;
        }

        ICollection<CardDraftPick> GetWeightsForCard(string userId, ICollection<int> grpIds)
        {
            var apiResponse = api.GetWeightsForCard(userId, grpIds);
            return Mapper.Map<ICollection<CardDraftPick>>(apiResponse.CardsForDraft);
        }

        public ICollection<CardDraftPick> ParseDraftMakePick(string userId, string text)
        {
            var match = regexDraftMakePickModel.Match(text);
            if (match.Success)
            {
                var model = JsonConvert.DeserializeObject<DraftMakePick>(match.Groups[1].Value);
                return GetWeightsForCard(userId, new[] { 68294, 69311 }); //model.draftPack.Cast<int>().ToArray());
            }

            return null;
        }
    }
}
