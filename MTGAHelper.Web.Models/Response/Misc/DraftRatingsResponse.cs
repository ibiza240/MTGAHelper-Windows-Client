using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class DraftRatingsResponse
    {
        public bool IsUpToDate { get; set; }
        public Dictionary<string, DraftRatings> Ratings { get; set; }
    }
}
