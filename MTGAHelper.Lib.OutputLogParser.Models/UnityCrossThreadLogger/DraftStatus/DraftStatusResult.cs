using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public interface IResultDraftPick
    {
        //List<string> DraftPack { get; }
        PayloadRaw<DraftMakePickRaw> Raw { get; set; }
    }

    public class DraftStatusResult : MtgaOutputLogPartResultBase<PayloadRaw<DraftMakePickRaw>>, IResultDraftPick//, IMtgaOutputLogPartResult<DraftMakePickRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.DraftStatus;

        ////// Same model so we reuse it
        ////public DraftMakePickRaw Raw { get; set; }
        //public List<string> DraftPack => Raw.draftPack;
    }
}
