using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class InvalidDeckFormatException : Exception
    {
        public InvalidDeckFormatException(string msg)
            : base(msg)
        {
        }

        public InvalidDeckFormatException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}
