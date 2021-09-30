using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.InventoryUpdated
{
    public class InventoryUpdatedResult : MtgaOutputLogPartResultBase<PayloadRaw<InventoryUpdatedRaw>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.InventoryUpdated;
    }
}