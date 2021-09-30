using MTGAHelper.Web.Models.Response.User.History;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class GetUserHistoryForDateResponse
    {
        public GetUserHistoryForDateResponseData History2 { get; }

        public GetUserHistoryForDateResponse(GetUserHistoryForDateResponseData history)
        {
            History2 = history;
        }
    }
}