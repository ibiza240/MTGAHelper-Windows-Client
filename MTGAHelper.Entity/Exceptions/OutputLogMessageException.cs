using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class OutputLogMessageException : Exception
    {
        public OutputLogMessageException(string msg)
            : base(msg)
        {
        }
    }
}
