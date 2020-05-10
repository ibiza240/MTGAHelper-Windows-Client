namespace MTGAHelper.Lib.OutputLogProgress
{
    public class ZoneTransfer
    {
        public int instanceId { get; set; }
        public long affectorId { get; set; }
        public int annotationId { get; set; }
        public Zone zone_src { get; set; }
        public Zone zone_dest { get; set; }
    }
}
