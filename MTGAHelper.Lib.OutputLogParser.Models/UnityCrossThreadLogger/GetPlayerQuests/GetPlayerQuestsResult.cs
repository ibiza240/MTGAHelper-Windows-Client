using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    //public interface IResultRaw<T>
    //{
    //    DateTime LogDateTime { get; }
    //    T Raw { get; set; }
    //}

    public class GetPlayerQuestsResult : MtgaOutputLogPartResultBase<PayloadRaw<ICollection<GetPlayerQuestRaw>>>//, IResultRaw<GetPlayerInventoryRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerInventory;

        //public new GetPlayerInventoryRaw Raw { get; set; }
    }
}
