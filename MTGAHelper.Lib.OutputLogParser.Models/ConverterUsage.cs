using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public class ConverterUsage
    {
        public string Converter { get; set; }
        public string LogTextKey { get; set; }
        public DateTime LastUsed { get; set; }
        public string Prefix { get; set; }
        public string Result { get; set; }
    }
}
