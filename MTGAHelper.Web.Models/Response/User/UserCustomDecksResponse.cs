using MTGAHelper.Web.UI.Model.Response.Dto;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.Config.Decks;

namespace MTGAHelper.Web.UI.Model.Response
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
