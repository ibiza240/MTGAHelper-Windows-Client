using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerCardsV3;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Readers.MTGAProLogger
{
    public class CollectionConverter : GenericConverter<GetPlayerCardsResult, PayloadRaw<Dictionary<int, int>>>, IMessageReaderMtgaProLogger
    {
        public override string LogTextKey => "**Collection**";
    }
}