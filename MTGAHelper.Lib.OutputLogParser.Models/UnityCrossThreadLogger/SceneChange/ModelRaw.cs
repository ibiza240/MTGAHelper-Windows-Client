namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class SceneChangeRaw
    {
        public string fromSceneName { get; set; }
        public string toSceneName { get; set; }
        public string initiator { get; set; }
        public string context { get; set; }
    }
}
