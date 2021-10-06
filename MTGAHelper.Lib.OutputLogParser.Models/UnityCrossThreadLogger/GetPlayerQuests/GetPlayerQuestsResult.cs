using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerQuests
{
    //public interface IResultRaw<T>
    //{
    //    DateTime LogDateTime { get; }
    //    T Raw { get; set; }
    //}

    public class GetPlayerQuestsResult : MtgaOutputLogPartResultBase<GetPlayerQuestsRaw>//, IResultRaw<GetPlayerInventoryRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerInventory;

        //public new GetPlayerInventoryRaw Raw { get; set; }
    }
}