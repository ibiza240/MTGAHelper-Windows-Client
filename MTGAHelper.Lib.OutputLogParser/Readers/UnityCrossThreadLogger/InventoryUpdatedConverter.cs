using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.InventoryUpdated;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class InventoryUpdatedConverter : GenericConverter<InventoryUpdatedResult, PayloadRaw<InventoryUpdatedRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Inventory.Updated";
    }
}