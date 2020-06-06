using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class InventoryUpdatedResult : MtgaOutputLogPartResultBase<PayloadRaw<InventoryUpdatedRaw>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.InventoryUpdated;
    }

}
