using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class DeckSubmitConverter : GenericConverter<DeckSubmitResult, PayloadRaw<DeckSubmitRaw>>
    {
        DeckListConverter deckConverter;

        public DeckSubmitConverter(DeckListConverter deckConverter)
        {
            this.deckConverter = deckConverter;
        }

        //public new IMtgaOutputLogPartResult<DeckSubmitRaw> ParseJson(string json)
        //{
        //    var raw = JsonConvert.DeserializeObject<DeckSubmitRaw>(json);
        //    var result = new DeckSubmitResult
        //    {
        //        Raw = raw,
        //        Deck = Mapper.Map<ConfigModelRawDeck>(raw.CourseDeck),
        //    };
        //    return result;
        //}
    }
}
