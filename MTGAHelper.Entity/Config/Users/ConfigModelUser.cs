using MTGAHelper.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public enum FormatEnum
    {
        Unknown,
        Standard,
        Historic
    }
}

namespace MTGAHelper.Lib.Config
{
    public enum UserPreferenceEnum
    {
        Unknown,
        ThemeIsDark,
        CollectionSetsOrder,
        LandsPickAll,
    }

    [Serializable]
    public class ConfigModelUser : IConfigModel
    {
        public const string USER_LOCAL = "localuser";
        public const string DEFAULT_SCRAPER = "mtggoldfish-meta-standard";

        public string PlayerName { get; set; } = "";
        public string Id { get; set; } = USER_LOCAL;

        public string LastUploadHash { get; set; } = "";

        public bool HasDeckIdsBeenRecoveredForEachMatch { get; set; }

        public DateTime DateCreatedUtc { get; set; } = DateTime.UtcNow;

        // Preferences, to refactor
        public bool ThemeIsDark { get; set; } = true;
        public string CollectionSetsOrder { get; set; } = "NewestFirst";
        public bool LandsPickAll { get; set; } = false;
        //public FormatEnum Format { get; set; } = FormatEnum.Standard;

        public DateTime LastLoginUtc { get; set; } = DateTime.UtcNow;
        public int NbLogin { get; set; }
        public Dictionary<RarityEnum, UserWeightDto> Weights { get; set; } = new Dictionary<RarityEnum, UserWeightDto>
        // Default weights but can be changed
        {
            {  RarityEnum.Mythic, new UserWeightDto(150, 1) },
            {  RarityEnum.RareLand, new UserWeightDto(0, 0) },
            {  RarityEnum.RareNonLand, new UserWeightDto(120, 1) },
            {  RarityEnum.Uncommon, new UserWeightDto(30, 1) },
            {  RarityEnum.Common, new UserWeightDto(40, 1) },
            {  RarityEnum.Other, new UserWeightDto(0, 0) },
        };

        //public MtgaUserProfile MtgaUserProfile { get; set; } = new MtgaUserProfile();

        public Dictionary<string, ConfigModelDeck> CustomDecks { get; set; } = new Dictionary<string, ConfigModelDeck>();

        public Dictionary<string, float> PriorityByDeckId { get; set; } = new Dictionary<string, float>();

        public List<string> ScrapersActive { get; set; } = new List<string>();

        public List<int> LandsPreference { get; set; } = new List<int>();

        public List<string> NotificationsInactive { get; set; } = new List<string>();

        public int NbDailyWinsExpected { get; set; } = 10;
        public int NbWeeklyWinsExpected { get; set; } = 15;

        //public ICollection<ConfigModelRawDeck> MtgaDecks { get; set; }

        //public ICollection<ConfigModelRankInfo> Ranks { get; set; }

        [JsonIgnore]
        public UserDataInMemoryModel DataInMemory { get; set; } = new UserDataInMemoryModel();

        public ConfigModelUser()
        {
            // For serialization
        }

        public ConfigModelUser(string userId)
        {
            // When creating a new user
            Id = userId;
            ScrapersActive = new List<string> { DEFAULT_SCRAPER };
        }

        public bool IsTracked(IDeck deck)
        {
            return (deck.ScraperType.Type == ScraperTypeEnum.UserCustomDeck || ScrapersActive.Contains(deck.ScraperType.Id)) &&
                   (PriorityByDeckId.ContainsKey(deck.Id) == false || PriorityByDeckId[deck.Id] > 0);
        }
    }
}
