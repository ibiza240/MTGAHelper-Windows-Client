using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.DraftStatus
{
    public interface IResultDraftPick
    {
        //List<string> DraftPack { get; }
        PayloadRaw<DraftPickStatusRaw> Raw { get; set; }
    }

    public class DraftResultBase : MtgaOutputLogPartResultBase<PayloadRaw<DraftPickStatusRaw>>, IResultDraftPick
    {
        //public PayloadRaw<DraftMakePickRaw> Raw { get; set; }
    }

    public class DraftStatusResult : DraftResultBase//, IMtgaOutputLogPartResult<DraftMakePickRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.DraftStatus;

        ////// Same model so we reuse it
        ////public DraftMakePickRaw Raw { get; set; }
        //public List<string> DraftPack => Raw.draftPack;
    }
}