using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerInventory;

namespace MTGAHelper.Lib.OutputLogParser.Readers.MTGAProLogger
{
    public class InventoryConverter : GenericConverter<GetPlayerInventoryResult, PayloadRaw<GetPlayerInventoryRaw>>, IMessageReaderMtgaProLogger
    {
        public override string LogTextKey => "**InventoryContent**";
    }
}