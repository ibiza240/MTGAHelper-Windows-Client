using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    //public interface IResultRaw<T>
    //{
    //    DateTime LogDateTime { get; }
    //    T Raw { get; set; }
    //}

    public class CompleteVaultResult : MtgaOutputLogPartResultBase<PayloadRaw<CompleteVaultRaw>>//, IResultRaw<GetPlayerInventoryRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerInventory;

        //public new GetPlayerInventoryRaw Raw { get; set; }
    }
}
