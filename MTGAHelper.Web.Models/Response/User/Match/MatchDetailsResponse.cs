using MTGAHelper.Web.Models.Response.User.History;

namespace MTGAHelper.Web.Models.Response.User.Match
{
    public class MatchDetailsResponse
    {
        public MatchDto Match { get; set; }

        public MatchDetailsResponse(MatchDto match)
        {
            this.Match = match;
        }
    }
}