using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class EmptyFileException : Exception
    {
        public EmptyFileException(string msg)
            : base(msg)
        {
        }
    }
}
