using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.Exceptions;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public abstract class ReaderMtgaOutputLogJsonParser
    {
        protected int GetPartTypeIndex(string part, string partType)
        {
            var index = part.IndexOf(partType);

            if (index >= 0)
            {
                //if (partType == ReaderMtgaOutputLogUnityCrossThreadLogger.UnityCrossThreadLogger_ClientConnected ||
                // partType == ReaderMtgaOutputLogUnityCrossThreadLogger.UnityCrossThreadLogger_DuelSceneGameStop ||
                // partType == ReaderMtgaOutputLogUnityCrossThreadLogger.UnityCrossThreadLogger_DuelSceneSideboardingStart ||
                // partType == ReaderMtgaOutputLogUnityCrossThreadLogger.UnityCrossThreadLogger_DuelSceneSideboardingStop)
                //{
                //    return part.IndexOf("Log.BI ");
                //}
                /*else*/ if (partType == GRE.ReaderMtgaOutputLogGreMatchToClient.GREMessageType_ConnectResp)
                {
                    return part.IndexOf('\n') - 2;
                }
            }

            return index;
        }

        string GetSubpart(string part, int partTypeStartIndex)
        {
            //if (part.Contains("Event.GetCombinedRankInfo")) System.Diagnostics.Debugger.Break();

            // Remove text before the converter key
            var subpart = part.Substring(partTypeStartIndex, part.Length - partTypeStartIndex);

            // Remove everything after this useless debugging log
            var endMaxIndex = subpart.IndexOf("(Filename:");
            if (endMaxIndex == -1)
                endMaxIndex = subpart.Length;

            //if (endMaxIndex > 0)
            return subpart.Substring(0, endMaxIndex);
            //else
            //{
            //    Log.Warning("Problem while trying to determine subpart: <{part}>, <{partTypeStartIndex}>", part.Trim(), partTypeStartIndex);
            //    return "{}";
            //}
        }

        protected string GetJson(string part, int partTypeStartIndex)
        {
            //try
            //{
            var subpart = GetSubpart(part, partTypeStartIndex);
            var jsonStartArray = subpart.IndexOf("[");
            var jsonStartObject = subpart.IndexOf("{");
            if (jsonStartArray < 0 && jsonStartObject < 0)
                throw new MtgaOutputLogInvalidJsonException("Invalid JSON");

            var lastChar = jsonStartArray != -1 && jsonStartArray < jsonStartObject ? "]" : "}";
            var lastCharIdx = subpart.LastIndexOf(lastChar);
            if (lastCharIdx < 0)
                throw new MtgaOutputLogInvalidJsonException("Invalid JSON");

            var jsonStart = new[] { jsonStartArray, jsonStartObject }.Where(i => i >= 0).Min();

            var json = subpart.Substring(jsonStart, lastCharIdx - jsonStart + 1);
            return json;
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //    return "";
            //    //throw;
            //}
        }

        protected string GetPartMessageSummarized(string part, int partTypeStartIndex)
        {
            var subpart = GetSubpart(part, partTypeStartIndex);

            // Remove everything before this
            var startIndex = subpart.IndexOf(":::");
            subpart = subpart.Substring(startIndex, subpart.Length - startIndex);

            return subpart;
        }
    }
}
