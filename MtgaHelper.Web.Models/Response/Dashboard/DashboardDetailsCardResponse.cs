using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DashboardDetailsCardResponse
    {
        public ICollection<DashboardDetailsCardDto> InfoByDeck { get; set; } = new DashboardDetailsCardDto[0];

        //public DashboardDetailsCardResponse()
        //{
        //}

        public static DashboardDetailsCardResponse Build(ICollection<ConfigModelDeck> configDecks,
            Card c, Dictionary<string, IDeck> decks, KeyValuePair<string, CardRequiredInfoByDeck>[] decksInfo)
        {
            var ret = new DashboardDetailsCardResponse();
            var dictDecks = configDecks.ToDictionary(i => i.Id, i => i);

            ret.InfoByDeck = decksInfo
                .Select(i => new DashboardDetailsCardDto
                {
                    DeckId = i.Key,
                    DeckName = decks[i.Key].Name,
                    NbMain = i.Value.ByCard[c.name].NbRequiredMain,
                    NbSideboard = i.Value.ByCard[c.name].NbRequiredSideboard,
                    DeckColor = decks[i.Key].GetColor(),
                    DeckDateCreated = dictDecks[i.Key].DateCreatedUtc,
                    DeckScraperTypeId = decks[i.Key].ScraperType.Id,
                })
                .ToArray();

            return ret;
        }
    }

    public class DashboardDetailsCardDto
    {
        public string DeckName { get; set; }
        public string DeckId { get; set; }
        public int NbMain { get; set; }
        public int NbSideboard { get; set; }

        public string DeckScraperTypeId { get; set; }
        public string DeckColor { get; set; }
        public DateTime? DeckDateCreated { get; set; }
    }
}
