using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class SceneChangeConverter : GenericConverter<SceneChangeResult, SceneChangeRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "Client.SceneChange";
    }
}