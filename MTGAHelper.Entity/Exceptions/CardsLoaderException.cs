using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class CardsLoaderException : Exception
    {
        public CardsLoaderException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}
