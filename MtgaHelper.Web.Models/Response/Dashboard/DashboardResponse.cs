using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Lib.CollectionDecksCompare;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DashboardResponse
    {

        public DashboardResponse(DashboardModel model)
        {
            Details = model.Details;
            Summary = model.Summary
                .Select(i => new KeyValuePair<string, InfoCardMissingSummary[]>(i.Key, i.Value))
                .OrderByDescending(i => i.Value.Sum(x => x.MissingWeight))
                .ToArray();
        }

        public CardMissingDetailsModel[] Details { get; set; }
        public KeyValuePair<string, InfoCardMissingSummary[]>[] Summary { get; set; }
    }
}
