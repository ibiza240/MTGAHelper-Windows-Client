using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class DraftStatusConverter : GenericConverter<DraftStatusResult, PayloadRaw<DraftMakePickRaw>>
    {
    }
}
