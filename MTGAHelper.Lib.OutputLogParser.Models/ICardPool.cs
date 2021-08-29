using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public interface ICardPool
    {
        List<int> CardPool { get; set; }
    }
}