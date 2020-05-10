using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public interface IResultCardPool
    {
        List<int> CardPool { get; }
    }

    public class GetEventPlayerCourseV2Result : MtgaOutputLogPartResultBase<PayloadRaw<GetEventPlayerCourseV2Raw>>, IResultCardPool
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetEventPlayerCourseV2;
        public List<int> CardPool => Raw.payload.CardPool;
    }
}
