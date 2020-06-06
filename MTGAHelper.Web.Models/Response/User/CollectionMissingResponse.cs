using System.Collections.Generic;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.UI.Model.Response.User
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
