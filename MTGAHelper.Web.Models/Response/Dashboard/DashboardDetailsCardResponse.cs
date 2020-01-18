using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config;
using Serilog;
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

        public static DashboardDetailsCardResponse Build(string userId, Dictionary<string, ConfigModelDeck> dictDecks,
            Card c, Dictionary<string, IDeck> decks, KeyValuePair<string, CardRequiredInfoByDeck>[] decksInfo,
            UtilColors utilColors)
        {
            var ret = new DashboardDetailsCardResponse();

            var missing = decksInfo.Where(i => decks.ContainsKey(i.Key) == false).ToArray();
            if (missing.Any())
            {
                Log.Error("User {userId} Error in DashboardDetailsCardResponse: Cannot find these decks: <{ids}>. Original ids: <{ids2}>",
                    userId, string.Join(",", missing.Select(x => x.Key)), string.Join(",", missing.Select(x => x.Value.DeckId)));
            }

            ret.InfoByDeck = decksInfo
                .Where(i => missing.Any(x => x.Key == i.Key) == false)
                .Select(i => new DashboardDetailsCardDto
                {
                    DeckId = i.Key,
                    DeckName = decks[i.Key].Name,
                    NbMain = i.Value.ByCard[c.name].NbRequiredMain,
                    NbSideboard = i.Value.ByCard[c.name].NbRequiredSideboard,
                    DeckColor = utilColors.FromDeck(decks[i.Key]),
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
