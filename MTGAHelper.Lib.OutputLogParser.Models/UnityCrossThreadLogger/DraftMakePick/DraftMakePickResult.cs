using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class DraftMakePickResult : MtgaOutputLogPartResultBase<PayloadRaw<DraftMakePickRaw>>, IResultDraftPick//, IMtgaOutputLogPartResult<DraftMakePickRaw>
    {
        //public List<string> DraftPack => Raw.draftPack;
    }
}
