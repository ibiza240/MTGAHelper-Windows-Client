using MTGAHelper.Web.Models.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User
{
    public class CollectionMissingResponse
    {
        public ICollection<CollectionCardDto> CardsMissing { get; }

        public CollectionMissingResponse(ICollection<CollectionCardDto> cardsMissing)
        {
            CardsMissing = cardsMissing;
        }
    }
}