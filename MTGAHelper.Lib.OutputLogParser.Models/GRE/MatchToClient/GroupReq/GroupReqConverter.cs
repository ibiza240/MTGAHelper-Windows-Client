using System;
using System.Linq;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GroupReq.Raw;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class GroupReqConverter : ConverterBase<GroupReqResult, GroupReqRaw>
    {
        protected override GroupReqResult CreateT(GroupReqRaw raw)
        {
            Enum.TryParse(raw.allowCancel, out AllowCancel allowCancel);
            Enum.TryParse(raw.groupReq.groupType, out GroupType groupType);
            Enum.TryParse(raw.groupReq.context, out GroupingContext context);

            var toLib = raw.groupReq.groupSpecs.Where(s => s.zoneType == ZoneSimpleEnum.ZoneType_Library.ToString()).ToList();
            var maxToTopLib = toLib.FirstOrDefault(spec => spec.subZoneType == SubZoneType.SubZoneType_Top.ToString())?.upperBound ?? 0;
            var maxToBottomLib = toLib.FirstOrDefault(spec => spec.subZoneType == SubZoneType.SubZoneType_Bottom.ToString())?.upperBound ?? 0;

            return new GroupReqResult(raw.systemSeatIds.FirstOrDefault(), allowCancel, raw.groupReq.instanceIds, groupType, context, maxToTopLib, maxToBottomLib)
            {
                Raw = raw
            };
        }
    }
}
