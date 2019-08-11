using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DashboardDetailsCardResponse
    {
        public ICollection<DashboardDetailsCardDto> InfoByDeck { get; set; } = new DashboardDetailsCardDto[0];

        public DashboardDetailsCardResponse()
        {
        }

        public DashboardDetailsCardResponse(Card c, Dictionary<string, IDeck> decks, KeyValuePair<string, CardRequiredInfoByDeck>[] decksInfo)
        {
            InfoByDeck = decksInfo
                .Select(i => new DashboardDetailsCardDto
                {
                    DeckId = i.Key,
                    DeckName = decks[i.Key].Name,
                    NbMain = i.Value.ByCard[c.name].NbRequiredMain,
                    NbSideboard = i.Value.ByCard[c.name].NbRequiredSideboard
                })
                .ToArray();
        }
    }

    public class DashboardDetailsCardDto
    {
        public string DeckName { get; set; }
        public string DeckId { get; set; }
        public int NbMain { get; set; }
        public int NbSideboard { get; set; }
    }
}
