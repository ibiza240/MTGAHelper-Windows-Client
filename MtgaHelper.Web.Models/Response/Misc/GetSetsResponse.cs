using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.Misc
{
    public class GetSetsResponse
    {
        public ICollection<SetInfo> Sets { get; set; }
    }

    public class SetInfo
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
        public int TotalCards { get; set; }
    }
}
