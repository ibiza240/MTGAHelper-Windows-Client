using System;

namespace MTGAHelper.Lib.OutputLogParser.Exceptions
{
    internal class MtgaOutputLogInvalidJsonException : Exception
    {
        public MtgaOutputLogInvalidJsonException(string msg)
            : base(msg)
        {

        }
    }
}
