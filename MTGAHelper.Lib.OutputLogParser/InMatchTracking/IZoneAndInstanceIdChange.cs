namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public interface IZoneAndInstanceIdChange
    {
        int OldInstanceId { get; }
        int NewInstanceId { get; }
        OwnedZone SrcZone { get; }
        OwnedZone DestZone { get; }
        int GrpId { get; }
    }
}
