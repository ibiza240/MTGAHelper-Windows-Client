using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class InventoryUpdatedResult : MtgaOutputLogPartResultBase<PayloadRaw<InventoryUpdatedRaw>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.InventoryUpdated;
    }

}
