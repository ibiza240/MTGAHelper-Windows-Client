using System;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class StateCard : IEquatable<StateCard>
    {
        public Guid Id { get; }
        public int GrpId { get; set; }

        public StateCard(int grpId)
        {
            GrpId = grpId;
            Id = Guid.NewGuid();
        }

        public bool Equals(StateCard other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((StateCard)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
