using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity.MtgaDeckStats;

namespace MTGAHelper.Web.Models.Response.User
{
    public class GetMtgaDeckSummaryResponse
    {
        public ICollection<MtgaDeckSummaryDto> Summary { get; set; }

        public GetMtgaDeckSummaryResponse(ICollection<MtgaDeckSummary> summary)
        {
            Summary = Mapper.Map<ICollection<MtgaDeckSummaryDto>>(summary);
        }
    }

    public class MtgaDeckSummaryDto
    {
        public string DeckId { get; set; }
        public string DeckImage { get; set; }
        public string DeckName { get; set; }
        public string DeckColor { get; set; }
        public string FirstPlayed { get; set; }
        public string LastPlayed { get; set; }
        public float WinRate { get; set; }
        public string WinRateFormat { get; set; }
        public int WinRateNbWin { get; set; }
        public int WinRateNbLoss { get; set; }
        public int WinRateNbOther { get; set; }
        public int WinRateNbMatches => WinRateNbWin + WinRateNbLoss + WinRateNbOther;
    }
}
