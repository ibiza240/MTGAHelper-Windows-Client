namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public interface ITrackedCard
    {
        /// <summary>Instance id should be immutable to ensure equality comparisons on InstId do not glitch</summary>
        int InstId { get; }
        int GrpId { get; }
        bool IsKnown { get; }
    }
}