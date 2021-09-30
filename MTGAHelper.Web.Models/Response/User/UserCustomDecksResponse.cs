using MTGAHelper.Entity.Config.Decks;
using MTGAHelper.Web.Models.Response.Deck;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.Response.User
{
    public class UserCustomDecksResponse
    {
        public ICollection<SimpleCustomDeckDto> Decks { get; set; }

        public UserCustomDecksResponse(ICollection<ConfigModelDeck> decks)
        {
            Decks = decks.Select(i => new SimpleCustomDeckDto(i.Id, i.Name)).ToArray();
        }
    }
}