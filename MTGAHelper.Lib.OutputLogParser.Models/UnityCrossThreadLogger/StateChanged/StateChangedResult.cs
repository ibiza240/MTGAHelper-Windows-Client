namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class StateChangedResult : MtgaOutputLogPartResultBase<StateChangedRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<RankUpdatedRaw>
    {
        public bool SignifiesMatchEnd => Raw.@new == 8;
    }

}
