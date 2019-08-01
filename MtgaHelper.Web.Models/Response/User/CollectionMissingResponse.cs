using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.UI.Model.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class CollectionMissingResponse
    {
        public ICollection<CollectionCardDto> CardsMissing { get; set; }

        public CollectionMissingResponse(ICollection<CardWithAmount> cardsMissing)
        {
            CardsMissing = Mapper.Map<ICollection<CollectionCardDto>>(cardsMissing);
        }
    }
}
