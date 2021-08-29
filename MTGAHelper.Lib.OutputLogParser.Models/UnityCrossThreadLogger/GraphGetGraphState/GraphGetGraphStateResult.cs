using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class GraphGetGraphStateResult : MtgaOutputLogPartResultBase<GraphGetGraphStateRaw>
    {
    }

    public partial class GraphGetGraphStateRaw
    {
        public Dictionary<string, dynamic> NodeStates { get; set; }
    }
}