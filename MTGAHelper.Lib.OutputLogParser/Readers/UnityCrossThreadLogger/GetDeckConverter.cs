using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetDeck;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetDeckConverter : GenericConverter<GetDeckResult, GetDeckRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Deck_GetDeck";
    }
}