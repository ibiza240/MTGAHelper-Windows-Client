using System.Text;

namespace MTGAHelper.Lib.OutputLogParser
{
    class ReadPartIntermediateResult
    {
        private readonly string _firstLine;
        private readonly StringBuilder _partLines = new StringBuilder().AppendLine();

        public ReadPartIntermediateResult(string firstLine, string prefix, string dateTimeStr, string restOfLine)
        {
            _firstLine = firstLine;
            Prefix = prefix;
            DateTimeStr = dateTimeStr;
            RestOfLine = restOfLine;
        }

        public string Part => (_firstLine + _partLines).Trim();
        public string Prefix { get; set; }
        public string DateTimeStr { get; }
        public string RestOfLine { get; }
        public string LeftToParse => (RestOfLine + _partLines).Trim();

        public void AddLine(string line)
        {
            _partLines.AppendLine(line);
        }
    }
}
