using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventJoinRequest
{
    public class GetDeckRequestResult : RequestParamsResult2<GetDeckRequestPayloadRaw>
    {
    }

    public partial class GetDeckRequestPayloadRaw
    {
        public string DeckId { get; set; }
    }
}