using System.Collections.Generic;

namespace MTGAHelper.Tracker.WPF.Config
{
    internal class ConfiguratorConfig
    {
        public string FolderCommunication { get; set; }
        public string FilepathConsole { get; set; }
        public string FolderData { get; set; }
        public string RatingsSource { get; set; }
        public ICollection<string> SetsSupported { get; set; }
    }
}
