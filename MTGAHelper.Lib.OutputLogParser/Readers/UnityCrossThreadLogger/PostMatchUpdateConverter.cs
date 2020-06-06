using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class PostMatchUpdateConverter : GenericConverter<PostMatchUpdateResult, PayloadRaw<PostMatchUpdateRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== PostMatch.Update";
    }
}
