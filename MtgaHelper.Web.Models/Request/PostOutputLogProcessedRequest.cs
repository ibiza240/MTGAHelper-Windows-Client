using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;

namespace MTGAHelper.Web.Models.Request
{
    public class PostOutputLogProcessedRequest
    {
        public PostOutputLogProcessedRequest(OutputLogResult result)
        {
            OutputLogResult = result;
        }

        public OutputLogResult OutputLogResult { get; set; }
    }
}
