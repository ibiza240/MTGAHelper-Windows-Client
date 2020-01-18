using MTGAHelper.Entity;

namespace MTGAHelper.Web.Models.Response.User
{
    public class DraftBoostersCriticalPointResultDto
    {
        public DraftBoostersCriticalPointResult Result { get; set; }
        public double AvgRaresCollected { get; set; }
        public double AvgMythicsCollected { get; set; }

        public DraftBoostersCriticalPointResultDto(
            DraftBoostersCriticalPointResult result,
            double avgRaresCollected, double avgMythicsCollected)
        {
            Result = result;
            AvgRaresCollected = avgRaresCollected;
            AvgMythicsCollected = avgMythicsCollected;
        }
    }
}
