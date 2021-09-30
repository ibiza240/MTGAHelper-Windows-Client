using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerInventory;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetPlayerInventoryConverter : GenericConverter<GetPlayerInventoryResult, PayloadRaw<GetPlayerInventoryRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== PlayerInventory.GetPlayerInventory";
    }
}