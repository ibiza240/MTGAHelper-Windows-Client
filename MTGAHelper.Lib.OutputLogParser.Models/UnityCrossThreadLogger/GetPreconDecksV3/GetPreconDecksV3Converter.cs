using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    //public class GetDecksListV3Converter : IReaderMtgaOutputLogJson
    //{
    //    DeckListConverter deckConverter;

    //    public GetDecksListV3Converter(DeckListConverter deckConverter)
    //    {
    //        this.deckConverter = deckConverter;
    //    }

    //    public IMtgaOutputLogPartResult ParseJson(string json)
    //    {
    //        //try
    //        //{
    //            var decksRaw = JsonConvert.DeserializeObject<IGetDeckListResultDeckRawCollection>(json);

    //            var result = new GetDecksListResult
    //            {
    //                //Decks = decksRaw.Select(i =>
    //                //{
    //                //    var d = Mapper.Map<GetDeckListResultDeck>(i);
    //                //    d.MainDeck = deckConverter.ConvertCards(i.mainDeck);
    //                //    d.Sideboard = deckConverter.ConvertCards(i.sideboard);
    //                //    return d;
    //                //}).ToArray()
    //                Raw = decksRaw
    //            };

    //            return result;
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    System.Diagnostics.Debugger.Break();
    //        //    return null;
    //        //}
    //    }
    //}
    public class GetPreconDecksV3Converter : GenericConverter<GetPreconDecksV3Result, PayloadRaw<ICollection<CourseDeckRaw>>>
    {
    }
}
