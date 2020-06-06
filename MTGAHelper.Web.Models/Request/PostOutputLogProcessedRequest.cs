using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Web.Models.Request
{
    public class PostOutputLogProcessedRequest
    {
        public PostOutputLogProcessedRequest()
        {
        }

        public PostOutputLogProcessedRequest(OutputLogResult result)
        {
            OutputLogResult = result;
        }

        public OutputLogResult OutputLogResult { get; set; }
    }

    public class PostOutputLogProcessedRequest2
    {
        public PostOutputLogProcessedRequest2()
        {
        }

        public PostOutputLogProcessedRequest2(OutputLogResult2 result2)
        {
            OutputLogResult2 = result2;
        }

        public OutputLogResult2 OutputLogResult2 { get; set; }
    }
}
