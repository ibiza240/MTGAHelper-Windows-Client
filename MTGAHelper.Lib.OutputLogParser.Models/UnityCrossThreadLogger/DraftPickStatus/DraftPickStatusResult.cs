using MTGAHelper.Entity.OutputLogParsing;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.DraftPickStatus
{
    public class DraftPickStatusResult : MtgaOutputLogPartResultBase<PayloadRaw<string>>
    {
        //public List<string> DraftPack => Raw.draftPack;
        public DraftPickStatusRaw Payload => JsonConvert.DeserializeObject<DraftPickStatusRaw>(Raw.payload);
    }
}