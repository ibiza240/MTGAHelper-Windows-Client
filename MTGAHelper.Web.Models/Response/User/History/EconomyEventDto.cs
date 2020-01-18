using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class EconomyEventDto
    {
        public string Context { get; set; }
        public DateTime DateTime { get; set; }
        public ICollection<EconomyEventChangeDto> Changes { get; set; } = new EconomyEventChangeDto[0];
        public ICollection<CardWithAmountDto> NewCards { get; set; } = new CardWithAmountDto[0];
    }

    public class EconomyEventChangeDto
    {
        public dynamic Amount { get; set; }
        public string Type { get; set; }
        public string Value { get; set; } = "";
    }
}
