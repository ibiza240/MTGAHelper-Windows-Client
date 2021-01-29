using MTGAHelper.Entity;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class ZoneTransferInfo2 : IZoneAndInstanceIdChange
    {
        public static ZoneTransferInfo2 Invalid { get; } = new ZoneTransferInfo2(0, OwnedZone.Unknown, OwnedZone.Unknown, "INVALID") { IsValid = false };

        public bool IsValid { get; private set; }
        public int OldInstanceId { get; private set; }
        public int NewInstanceId { get; }
        public OwnedZone SrcZone { get; }
        public OwnedZone DestZone { get; }
        public string Category { get; }
        public Card Card { get; private set; }
        public int GrpId { get; private set; }

        public ZoneTransferInfo2(int newInstanceId, OwnedZone srcZone, OwnedZone destZone, string category = "")
        {
            IsValid = true;
            NewInstanceId = newInstanceId;
            SrcZone = srcZone;
            DestZone = destZone;
            Category = category;
        }

        public ZoneTransferInfo2 WithOldId(int oldInstanceId)
        {
            OldInstanceId = oldInstanceId;
            return this;
        }

        public ZoneTransferInfo2 WithGrpId(Card card, int grpId)
        {
            Card = card;
            GrpId = grpId;
            return this;
        }

        public override string ToString()
        {
            var cardInfo = Card == null ? string.Empty : $"'{Card.name}'({GrpId}) ";
            return IsValid
                ? $"{cardInfo}from {OldInstanceId} in {SrcZone} to {NewInstanceId} in {DestZone} ({Category})"
                : "Invalid move";
        }
    }
}
