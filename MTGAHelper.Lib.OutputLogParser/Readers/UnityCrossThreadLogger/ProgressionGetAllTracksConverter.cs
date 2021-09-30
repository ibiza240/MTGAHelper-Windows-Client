using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.ProgressionGetAllTracks;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class ProgressionGetAllTracksConverter : GenericConverter<ProgressionGetAllTracksResult, PayloadRaw<ICollection<ProgressionGetAllTracksRaw>>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Progression.GetAllTracks";
    }
}