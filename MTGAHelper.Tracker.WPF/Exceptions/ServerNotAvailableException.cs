using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Exceptions
{
    public class ServerNotAvailableException : Exception
    {
        public ServerNotAvailableException()
        {
        }

        public ServerNotAvailableException(Exception ex)
            : base("Remote server not available", ex)
        {
        }
    }
}
