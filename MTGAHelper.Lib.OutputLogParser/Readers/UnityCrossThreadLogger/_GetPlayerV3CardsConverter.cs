//using MTGAHelper.Entity.OutputLogParsing;
//using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerCardsV3;
//using System.Collections.Generic;

//namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
//{
//    //public class GetPlayerV3CardsConverter : IReaderMtgaOutputLogJson
//    //{
//    //    ICollection<Card> allCards;
//    //    public GetPlayerV3CardsConverter(CacheSingleton<ICollection<Card>> cacheCards)
//    //    {
//    //        this.allCards = cacheCards.Get();
//    //    }

//    //    public IMtgaOutputLogPartResult ParseJson(string json)
//    //    {
//    //        //var reader = new ReaderCollection().Init(allCards);
//    //        //var collection = reader.LoadCollection(json);
//    //        var result = new GetPlayerCardsResult
//    //        {
//    //            //Cards = reader.ResultRaw,
//    //            Cards = JsonConvert.DeserializeObject<Dictionary<int, int>>(json),
//    //        };
//    //        return result;
//    //    }
//    //}
//    public class GetPlayerV3CardsConverter : GenericConverter<GetPlayerCardsResult, PayloadRaw<Dictionary<int, int>>>, IMessageReaderUnityCrossThreadLogger
//    {
//        public override string LogTextKey => "<== PlayerInventory.GetPlayerCardsV3";
//    }
//}