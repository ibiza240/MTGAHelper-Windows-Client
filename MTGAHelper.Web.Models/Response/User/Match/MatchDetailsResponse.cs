using AutoMapper;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Web.UI.Model.Response.User.History;

namespace MTGAHelper.Web.Models.Response.User.Match
{
    public class MatchDetailsResponse
    {
        public MatchDto Match { get; set; }

        public MatchDetailsResponse(MatchResult match)
        {
            this.Match = Mapper.Map<MatchDto>(match);
        }
    }
}
