using System.Collections.Generic;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.Models.Response.User
{
    public class DraftRaredraftingInfoResponse
    {
        public ICollection<CardCompareInfo> Data { get; set; }

        public DraftRaredraftingInfoResponse()
        {
        }

        public DraftRaredraftingInfoResponse(ICollection<CardCompareInfo> data)
        {
            Data = data;
        }
    }
}
