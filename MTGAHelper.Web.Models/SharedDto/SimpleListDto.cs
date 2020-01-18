using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.SharedDto
{
    public class SimpleListDto
    {
        public ICollection<string> List { get; set; }

        public SimpleListDto(ICollection<string> list)
        {
            List = list;
        }
    }
}
