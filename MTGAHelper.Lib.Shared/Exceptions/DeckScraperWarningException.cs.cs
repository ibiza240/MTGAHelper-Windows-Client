using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class DeckScraperWarningException : Exception
    {
        public DeckScraperWarningException(string msg)
            : base(msg)
        {
        }

        public DeckScraperWarningException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}