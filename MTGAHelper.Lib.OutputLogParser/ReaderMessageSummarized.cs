using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class ReaderMessageSummarized : IReaderMtgaOutputLogPart
    {
        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            ////e.g.
            ////::: GREMessageType_GameStateMessage
            ////::GameObject Count = 9
            ////:: Annotation Count = 11
            ////::: GREMessageType_GameStateMessage
            ////::GameObject Count = 0
            ////:: Annotation Count = 13
            ////::: GREMessageType_IntermissionReq

            //var results = part.Split('\n')
            //    .Select(i => i.Trim())
            //    .Where(i => i.StartsWith(":::"))
            //    .Select(i => CreateEmptyMessage(i))
            //    .ToArray();

            //return results;

            return new[] { new SummarizedMatchResult() };
        }

        //private IMtgaOutputLogPartResult CreateEmptyMessage(string line)
        //{
        //    IMtgaOutputLogPartResult result = null;
        //    if (line.Contains(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_ConnectResp))
        //        result = new ConnectRespResult();

        //    else if (line.Contains(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_GameStateMessage))
        //        result = new GameStateMessageResult();

        //    else if (line.Contains(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_IntermissionReq))
        //        result = new IntermissionReqResult();

        //    else if (line.Contains(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_MulliganReq))
        //        result = new MulliganReqResult();

        //    else if (line.Contains(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_SubmitDeckReq))
        //        result = new SubmitDeckReqResult();

        //    else
        //        result = new IgnoredMatchResult();

        //    result.IsMessageSummarized = true;
        //    return result;
        //}
    }
}
