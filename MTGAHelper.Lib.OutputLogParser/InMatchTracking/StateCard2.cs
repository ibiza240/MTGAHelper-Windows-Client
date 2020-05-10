using System.Collections.Generic;
using System.Diagnostics;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class StateCard2
    {
        public StateCard2(int grpId, OwnedZone zone)
        {
            GrpId = grpId;
            IsOpponentsCard = zone.IsOpponentZone();
            zoneHistory.Push(zone);
        }

        readonly SortedSet<int> knownInstanceIds = new SortedSet<int>();
        public int InstId { get; private set; }

        public int GrpId { get; }

        public bool IsKnown => GrpId != 0;
        public bool IsOpponentsCard { get; }

        readonly Stack<OwnedZone> zoneHistory = new Stack<OwnedZone>();
        public OwnedZone CurrentZone => zoneHistory.Peek();


        public StateCard2 UpdateInstanceId(int newInstanceId)
        {
            knownInstanceIds.Add(newInstanceId);
            InstId = newInstanceId;
            return this;
        }

        public StateCard2 MoveToZone(OwnedZone newZone, OwnedZone fromZone = OwnedZone.Unknown)
        {
            if (fromZone != OwnedZone.Unknown && fromZone != CurrentZone)
            {
                Log.Warning("Card {InstId}({GrpId}) expected to move from {fromZone} to {newZone} but was in zone {CurrentZone}",
                    InstId,
                    GrpId,
                    fromZone,
                    newZone,
                    CurrentZone);
                Debugger.Break();
            }

            zoneHistory.Push(newZone);
            return this;
        }

        public override string ToString()
        {
            return $"{InstId} (grpId {GrpId}) in zone {CurrentZone}";
        }
    }
}