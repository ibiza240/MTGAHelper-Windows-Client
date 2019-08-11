using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class DeckScraperWarningException : Exception
    {
        public DeckScraperWarningException(string msg)
            : base(msg)
        {
        }
    }
}