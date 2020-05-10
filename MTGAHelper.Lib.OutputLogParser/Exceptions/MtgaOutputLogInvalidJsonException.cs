using System;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.Exceptions
{
    internal class MtgaOutputLogInvalidJsonException : Exception
    {
        public MtgaOutputLogInvalidJsonException(string msg)
            : base(msg)
        {

        }
    }
}
