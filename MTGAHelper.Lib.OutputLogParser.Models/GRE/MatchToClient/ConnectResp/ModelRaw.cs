using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.ConnectResp
{
    public class ConnectRespRaw : GreMatchToClientSubMessageBase
    {
        public ConnectResp connectResp { get; set; }
    }

    public class Stop
    {
        public string stopType { get; set; }
        public string appliesTo { get; set; }
        public string status { get; set; }
    }

    public class TransientStop
    {
        public string stopType { get; set; }
        public string appliesTo { get; set; }
        public string status { get; set; }
    }

    public class Settings
    {
        public List<Stop> stops { get; set; }
        public string autoPassOption { get; set; }
        public string graveyardOrder { get; set; }
        public string manaSelectionType { get; set; }
        public string defaultAutoPassOption { get; set; }
        public string smartStopsSetting { get; set; }
        public string autoTapStopsSetting { get; set; }
        public string autoOptionalPaymentCancellationSetting { get; set; }
        public List<TransientStop> transientStops { get; set; }
    }

    public class DeckMessage : IDeckMessage
    {
        public List<int> deckCards { get; set; }
        public List<int> sideboardCards { get; set; }
        public List<int> commanderCards { get; set; }
    }

    public class ConnectResp
    {
        public string status { get; set; }
        public int majorVer { get; set; }
        public int revisionVer { get; set; }
        public int buildVer { get; set; }
        public string protoVer { get; set; }
        public Settings settings { get; set; }
        public DeckMessage deckMessage { get; set; }
    }
}
