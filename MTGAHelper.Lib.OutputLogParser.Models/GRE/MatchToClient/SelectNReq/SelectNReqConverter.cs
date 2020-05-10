using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.SelectNReq.Raw;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class SelectNReqConverter : ConverterBase<SelectNReqResult, SelectNReqRaw>
    {
        protected override SelectNReqResult CreateT(SelectNReqRaw raw)
        {
            Enum.TryParse(raw.allowCancel, out AllowCancel allowCancel);
            Enum.TryParse(raw.selectNReq.optionType, out OptionType optionType);
            Enum.TryParse(raw.selectNReq.idType, out IdType idType);

            var minTake = raw.selectNReq.minSel;
            var maxTake = raw.selectNReq.maxSel;

            var ids = raw.selectNReq.unfilteredIds ?? raw.selectNReq.ids ?? new List<int>(0);

            return new SelectNReqResult(raw.systemSeatIds.FirstOrDefault(), allowCancel, ids, optionType, idType, minTake, maxTake)
            {
                Raw = raw
            };
        }
    }
}
