using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class OldDataException : Exception
    {
        public OldDataException(string msg)
            : base(msg)
        {
        }
    }
}
