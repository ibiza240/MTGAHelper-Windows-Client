using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class DeckFileAlreadyExistsException : Exception
    {
        public DeckFileAlreadyExistsException(string msg)
            : base(msg)
        {
        }
    }
}
