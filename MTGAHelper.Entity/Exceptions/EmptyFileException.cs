using System;
using System.Collections.Generic;
using System.Text;

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
