using System;

namespace MTGAHelper.Lib.OutputLogParser.Exceptions
{
    internal class MtgaOutputLogUnknownMessageException : Exception
    {
        public MtgaOutputLogUnknownMessageException(string msg)
            :base(msg)
        {

        }
    }

    internal class MtgaOutputLogUnknownMessageMatchException : MtgaOutputLogUnknownMessageException
    {
        public MtgaOutputLogUnknownMessageMatchException(string msg)
            : base(msg)
        {

        }
    }
}
