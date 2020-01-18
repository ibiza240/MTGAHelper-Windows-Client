using System;
using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity.MtgaDeckStats;

namespace MTGAHelper.Web.Models.Response.User
{
    public class GetMtgaDeckAnalysisResponse
    {
        public MtgaDeckAnalysisDto Analysis { get; set; }

        public GetMtgaDeckAnalysisResponse(MtgaDeckAnalysis analysis)
        {
            Analysis = Mapper.Map<MtgaDeckAnalysisDto>(analysis);
        }
    }

    public class MtgaDeckAnalysisDto
    {
        public string DeckId { get; set; }
        public string DeckImage { get; set; }
        public string DeckName { get; set; }
        public ICollection<MtgaDeckAnalysisMatchInfoDto> MatchesInfo { get; set; }
    }

    public class MtgaDeckAnalysisMatchInfoDto
    {
        public DateTime StartDateTime { get; set; }
        public string EventName { get; set; }
        public string OpponentColors { get; set; }
        public string OpponentRank { get; set; }
        public string FirstTurn { get; set; }
        public string Outcome { get; set; }
        public int Mulligans { get; set; }
        public int OpponentMulligans { get; set; }
    }
}
