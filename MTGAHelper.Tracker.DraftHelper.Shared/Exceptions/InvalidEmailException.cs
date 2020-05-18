using System;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Exceptions
{
    public class InvalidEmailException : InvalidOperationException
    {
        public InvalidEmailException(string message)
            : base(message)
        {
        }
    }
}
