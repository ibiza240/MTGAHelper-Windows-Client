using MTGAHelper.Entity.UserHistory;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class EconomyResponse
    {
        public Dictionary<string, Dictionary<EconomyEventChangeEnum, EconomyResponseSummary>> DataBySetThenInventoryType { get; set; }
    }

    public class EconomyResponseSummary
    {
        public ICollection<EconomyResponseItem> Gains { get; set; }
        public ICollection<EconomyResponseItem> Spendings { get; set; }
    }

    public class EconomyResponseItem
    {
        public string Context { get; set; }
        public float Variation { get; set; }
        public string InventoryItemType { get; set; }
        public string Value { get; set; }
    }
}
