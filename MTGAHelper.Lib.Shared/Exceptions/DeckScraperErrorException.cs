using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class DeckScraperErrorException : Exception
    {
        public DeckScraperErrorException(string msg)
            : base(msg)
        {
        }

        public DeckScraperErrorException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}
