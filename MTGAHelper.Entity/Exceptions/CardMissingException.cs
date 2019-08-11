using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class CardMissingException : Exception
    {
        public CardMissingException(string msg)
            : base(msg)
        {
        }
    }
}
