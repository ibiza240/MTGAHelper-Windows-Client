//using System.Collections.Generic;
//using MTGAHelper.Entity.OutputLogParsing;
//using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

//namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
//{
//    //public class GetDecksListV3Converter : IReaderMtgaOutputLogJson
//    //{
//    //    DeckListConverter deckConverter;

//    //    public GetDecksListV3Converter(DeckListConverter deckConverter)
//    //    {
//    //        this.deckConverter = deckConverter;
//    //    }

//    //    public IMtgaOutputLogPartResult ParseJson(string json)
//    //    {
//    //        //try
//    //        //{
//    //            var decksRaw = JsonConvert.DeserializeObject<IGetDeckListResultDeckRawCollection>(json);

//    //            var result = new GetDecksListResult
//    //            {
//    //                //Decks = decksRaw.Select(i =>
//    //                //{
//    //                //    var d = Mapper.Map<GetDeckListResultDeck>(i);
//    //                //    d.MainDeck = deckConverter.ConvertCards(i.mainDeck);
//    //                //    d.Sideboard = deckConverter.ConvertCards(i.sideboard);
//    //                //    return d;
//    //                //}).ToArray()
//    //                Raw = decksRaw
//    //            };

//    //            return result;
//    //        //}
//    //        //catch (Exception ex)
//    //        //{
//    //        //    System.Diagnostics.Debugger.Break();
//    //        //    return null;
//    //        //}
//    //    }
//    //}
//    public class GetDecksListV3Converter : GenericConverter<GetDecksListResult, PayloadRaw<ICollection<Entity.OutputLogParsing.CourseDeckRaw>>>, IMessageReaderUnityCrossThreadLogger
//    {
//        public override string LogTextKey => "<== Deck.GetDeckListsV3";
//    }
//}