using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser.Models;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class GameCardInZone : CardIds
    {
        public int OwnerSeatId { get; }
        public OwnedZone Zone { get; }

        public GameObjectType GameObjectType { get; }
        public Visibility Visibility { get; }
        public Card Card { get; }

        public GameCardInZone(
            int instanceId,
            int ownerSeatId,
            OwnedZone zone,
            int grpId,
            GameObjectType gameObjectType,
            Visibility visibility,
            Card card) : base(instanceId, grpId)
        {
            OwnerSeatId = ownerSeatId;
            Zone = zone;
            GameObjectType = gameObjectType;
            Visibility = visibility;
            Card = card;
        }

        public GameCardInZone MovedTo(int newInstId, OwnedZone destZone, int? grpId = null)
        {
            if (grpId != null && grpId != GrpId)
            {
                Log.Warning($"changing grpId from {GrpId} to {grpId}");
            }
            return new GameCardInZone(newInstId, OwnerSeatId, destZone, grpId ?? GrpId, GameObjectType, Visibility, Card);
        }

        public override string ToString()
        {
            var ret = $"{InstId} ({GrpId} {OwnerSeatId}) in {Zone}";
            return GameObjectType == GameObjectType.GameObjectType_Card
                ? ret
                : $"[{GameObjectType.ShortString()}] " + ret;
        }
    }
}