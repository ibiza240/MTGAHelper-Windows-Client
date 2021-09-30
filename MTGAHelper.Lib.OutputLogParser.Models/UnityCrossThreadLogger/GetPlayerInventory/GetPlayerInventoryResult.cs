using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerInventory
{
    //public interface IResultRaw<T>
    //{
    //    DateTime LogDateTime { get; }
    //    T Raw { get; set; }
    //}

    public class GetPlayerInventoryResult : MtgaOutputLogPartResultBase<PayloadRaw<GetPlayerInventoryRaw>>//, IResultRaw<GetPlayerInventoryRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerInventory;

        //public new GetPlayerInventoryRaw Raw { get; set; }
    }
}