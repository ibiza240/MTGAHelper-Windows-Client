namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class CardIds : ITrackedCard
    {
        public int InstId { get; }
        public virtual int GrpId { get; }
        public virtual bool IsKnown => GrpId != 0;

        protected CardIds(int instanceId)
        {
            InstId = instanceId;
        }
        public CardIds(int instanceId, int grpId) : this(instanceId)
        {
            GrpId = grpId;
        }

        public override string ToString()
        {
            return IsKnown ? $"{InstId} (grpId {GrpId})" : $"{InstId}";
        }
    }
}