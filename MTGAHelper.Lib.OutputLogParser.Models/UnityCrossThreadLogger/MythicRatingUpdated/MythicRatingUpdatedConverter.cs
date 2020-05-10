using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    //public class MythicRatingUpdatedConverter : IReaderMtgaOutputLogJson
    //{
    //    public IMtgaOutputLogPartResult ParseJson(string json)
    //    {
    //        var raw = JsonConvert.DeserializeObject<RankUpdatedRaw>(json);
    //        var result = new RankUpdatedResult
    //        {
    //            Raw = raw,
    //        };
    //        return result;
    //    }
    //}
    public class MythicRatingUpdatedConverter : GenericConverter<MythicRatingUpdatedResult, PayloadRaw<MythicRatingUpdatedRaw>>
    {
    }
}
