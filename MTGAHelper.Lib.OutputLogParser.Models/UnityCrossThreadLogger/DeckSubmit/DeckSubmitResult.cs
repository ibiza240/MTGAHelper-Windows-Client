using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class DeckSubmitResult : MtgaOutputLogPartResultBase<PayloadRaw<DeckSubmitRaw>>//, ITagMatchResult//, IMtgaOutputLogPartResult<DeckSubmitRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.DeckSubmit;

        //public new DeckSubmitRaw Raw { get; set; }

        public ConfigModelRawDeck Deck { get; set; }
    }

}
