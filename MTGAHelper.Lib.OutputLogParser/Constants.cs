using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Lib.OutputLogParser
{
    public static class Constants
    {
        public const string LOGTEXTKEY_UNKNOWN = "**UNKNOWN**";
        public const string LOGTEXTKEY_UNKNOWN_OutputLogGre = "**'Match to ' or ' to Match'**";

        public const string PREFIX_MESSAGESUMMARIZED = "Message summarized";   // because one or more GameStateMessages exceeded the 50 GameObject or 50 Annotation limit.
    }
}
