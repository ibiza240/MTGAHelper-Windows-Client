using System;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.Exceptions
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
