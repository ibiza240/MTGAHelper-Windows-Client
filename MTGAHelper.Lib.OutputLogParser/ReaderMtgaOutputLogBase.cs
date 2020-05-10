using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public class ReaderMtgaOutputLogBase
    {
        public Dictionary<Type, ICollection<IMtgaOutputLogPartResult>> Results = new Dictionary<Type, ICollection<IMtgaOutputLogPartResult>>();
        public Dictionary<string, ICollection<IMtgaOutputLogPartResult>> Ignored = new Dictionary<string, ICollection<IMtgaOutputLogPartResult>>();
        public Dictionary<string, ICollection<IMtgaOutputLogPartResult>> Unknown = new Dictionary<string, ICollection<IMtgaOutputLogPartResult>>();

        public void SaveResult(IMtgaOutputLogPartResult r)
        {
            // Not in a match
            if (r is IgnoredResult || r is UnknownResult)
            {
                var resultsList = r.GetType() == typeof(IgnoredResult) ? Ignored : Unknown;
                if (resultsList.ContainsKey(r.Prefix) == false) resultsList.Add(r.Prefix, new List<IMtgaOutputLogPartResult>());
                resultsList[r.Prefix].Add(r);
            }
            else
            {
                if (Results.ContainsKey(r.GetType()) == false) Results.Add(r.GetType(), new List<IMtgaOutputLogPartResult>());
                Results[r.GetType()].Add(r);
            }
        }
    }

    internal class ReaderMtgaOutputLogMatch : ReaderMtgaOutputLogBase
    {
        public string MatchId { get; private set; }

        public ReaderMtgaOutputLogMatch(string matchId)
        {
            MatchId = matchId;
        }
    }

}
