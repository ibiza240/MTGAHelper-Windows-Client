using MTGAHelper.Lib.Config.Users;
using System.Linq;

namespace MTGAHelper.Lib.UserHistory
{
    public class DateSnapshot
    {
        public DateSnapshot()
        {
        }

        public DateSnapshot(DateSnapshotInfo snapshotData)
        {
            Info = snapshotData;
        }

        public DateSnapshotInfo Info { get; set; }

        public DateSnapshotDiff Diff { get; set; } = new DateSnapshotDiff();

        //public string ConstructedRank
        //{
        //    get
        //    {
        //        var r = Info.RankInfo.First(y => y.Format == ConfigModelRankInfoFormatEnum.Constructed);
        //        return r.ToString();
        //    }
        //}
        //public string LimitedRank
        //{
        //    get
        //    {
        //        var r = Info.RankInfo.First(y => y.Format == ConfigModelRankInfoFormatEnum.Limited);
        //        return r.ToString();
        //    }
        //}
    }

}
