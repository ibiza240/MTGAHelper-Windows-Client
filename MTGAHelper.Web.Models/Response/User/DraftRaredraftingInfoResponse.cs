using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;

namespace MTGAHelper.Web.Models.Response.User
{
    public class DraftRaredraftingInfoResponse
    {
        public ICollection<CardCompareInfo> Data { get; set; }

        public DraftRaredraftingInfoResponse()
        {
        }

        public DraftRaredraftingInfoResponse(CardsMissingResult data)
        {
            Data = data.ByCard
                //.Select(i =>
                //{
                //    var nbDecksMain = i.Value.ByDeck.Count(x => x.Value.NbMissingMain > 0);

                //    return new CardCompareInfo
                //    {
                //        GrpId = i.Value.Card.grpId,
                //        MissingWeight = i.Value.MissingWeight,
                //        NbDecksMain = nbDecksMain,
                //        NbDecksSideboardOnly = i.Value.NbDecks - nbDecksMain,
                //        NbMissing = i.Value.NbMissing,
                //    };
                //})
                .Select(i => Mapper.Map<CardCompareInfo>(i.Value))
                .ToArray();
        }
    }
}
