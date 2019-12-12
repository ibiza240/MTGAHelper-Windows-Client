using System.Collections.Generic;
using System.IO;

namespace MTGAHelper.Lib.Config
{
    public enum ConfigAppFeatureEnum
    {
        ParseMatches,
        //RecoverDeckIdFromDeckNameForEachMatchPlayed_V0_21
    }

    public class ConfigModelSetInfo
    {
        public int NbCards { get; set; }
        public ICollection<string> Formats { get; set; }
    }

    public class ConfigModelApp
    {
        //public const string CURRENT_SET = "ELD";

        public string VersionTrackerClient { get; set; }
        public string FolderData { get; set; } = "./data";
        public string FolderLogs { get; set; } = "./logs";
        public string FolderDlls { get; set; } = "./wwwroot";
        public string FolderInvalidZips { get; set; } = "./FailedZipFiles";
        public string FolderUserMessages { get; set; } = "./UserMessages";

        //public string FolderDataDecks => Path.Combine(FolderData, "decks");
        public string FolderDataConfigUsers => Path.Combine(FolderData, "configusers");
        public string FolderDataAccounts => Path.Combine(FolderData, "accounts");
        public string FolderDataGlobal => Path.Combine(FolderData, "global");

        //public List<string> CardsObtainableOnlyByCrafting { get; set; } = new List<string>();

        //public ConfigLogWithGmail LogWithGmail { get; set; } = new ConfigLogWithGmail();

        public Dictionary<string, bool> Features { get; set; } = new Dictionary<string, bool>();

        public Dictionary<string, string> TrackerClientMessages { get; set; } = new Dictionary<string, string>();

        public List<ConfigChangelog> Changelog { get; set; } = new List<ConfigChangelog>();

        public Dictionary<string, ConfigModelSetInfo> InfoBySet { get; set; }

        public string CurrentSet { get; set; }

        //public void BuildEmailConfig()
        //{
        //    var creds = new string[] { "", "" };
        //    if (File.Exists("EmailCredentials.txt"))
        //        creds = File.ReadAllText("EmailCredentials.txt").Split('\t');

        //    LogWithGmail.Username = creds[0];
        //    LogWithGmail.Password = creds[1];
        //}

        public ConfigModelApp()
        {
        }

        public bool IsFeatureEnabled(ConfigAppFeatureEnum feature)
        {
            var f = feature.ToString();
            if (Features.ContainsKey(f) == false || Features[f])
                return true;

            return false;
        }
    }

    //public class ConfigLogWithGmail
    //{
    //    public string From { get; set; }
    //    public string Username { get; set; }
    //    public string Password { get; set; }
    //    public string To { get; set; }
    //}

    public class ConfigChangelog
    {
        public string Date { get; set; }
        public string Version { get; set; }
        public List<ConfigChangelogItem> Changes { get; set; } = new List<ConfigChangelogItem>();
    }

    public class ConfigChangelogItem
    {
        public string Section { get; set; }
        public string Description { get; set; }
    }
}
