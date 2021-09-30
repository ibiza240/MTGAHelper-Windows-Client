using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response
{
    public class KeyValueResponse
    {
        public Dictionary<string, string> Items { get; set; }

        public KeyValueResponse(Dictionary<string, string> items)
        {
            Items = items;
        }
    }
}