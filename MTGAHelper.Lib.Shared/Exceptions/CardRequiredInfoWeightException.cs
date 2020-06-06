using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class CardRequiredInfoWeightException : Exception
    {
        public CardRequiredInfoWeightException(string msg)
            : base(msg)
        {
        }
    }
}
