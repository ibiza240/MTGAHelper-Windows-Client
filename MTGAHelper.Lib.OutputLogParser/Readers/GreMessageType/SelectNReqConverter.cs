using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.SelectNReq;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class SelectNReqConverter : ConverterBase<SelectNReqResult, SelectNReqRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_SelectNReq;

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
