using System;
using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class StartHookResult : MtgaOutputLogPartResultBase<StartHookRaw>//, IMtgaOutputLogPartResult<ICollection<GetDeckListResultDeckRaw>>
    {
    }

    public partial class StartHookRaw
    {
        public InventoryInfo InventoryInfo { get; set; }
        public DeckSummary[] DeckSummaries { get; set; }
        public object[] SystemMessages { get; set; }
        public object SensitiveArt { get; set; }
        public Format[] Formats { get; set; }
        public Cosmetics AvailableCosmetics { get; set; }
        public PreferredCosmetics PreferredCosmetics { get; set; }
        public long DeckLimit { get; set; }
        public TokenDefinition[] TokenDefinitions { get; set; }
        public KillSwitchNotification KillSwitchNotification { get; set; }
        public CardMetadataInfo CardMetadataInfo { get; set; }
        public ClientPeriodicRewards ClientPeriodicRewards { get; set; }
    }

    public partial class Cosmetics
    {
        public Pet[] ArtStyles { get; set; }
        public Avatar[] Avatars { get; set; }
        public Pet[] Pets { get; set; }
        public Avatar[] Sleeves { get; set; }
        public Emote[] Emotes { get; set; }
    }

    public partial class Pet
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public long? ArtId { get; set; }
        public string Variant { get; set; }
        public string Name { get; set; }
    }

    public partial class Avatar
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public partial class Emote
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Page { get; set; }
        public string FlipType { get; set; }
        public string Category { get; set; }
        public string Treatment { get; set; }
    }

    public partial class CardMetadataInfo
    {
        public long[] NonCraftableCardList { get; set; }
        public long[] NonCollectibleCardList { get; set; }
        public object[] UnreleasedSets { get; set; }
    }

    public partial class ClientPeriodicRewards
    {
        public long DailyRewardSequenceId { get; set; }
        public DateTimeOffset DailyRewardResetTimestamp { get; set; }
        public long WeeklyRewardSequenceId { get; set; }
        public DateTimeOffset WeeklyRewardResetTimestamp { get; set; }
        public Dictionary<string, DailyRewardChestDescription> DailyRewardChestDescriptions { get; set; }
        public Dictionary<string, WeeklyRewardChestDescription> WeeklyRewardChestDescriptions { get; set; }
    }

    public partial class DailyRewardChestDescription
    {
        public string Image1 { get; set; }
        public object Image2 { get; set; }
        public object Image3 { get; set; }
        public string Prefab { get; set; }
        public object ReferenceId { get; set; }
        public string HeaderLocKey { get; set; }
        public string DescriptionLocKey { get; set; }
        public long? Quantity { get; set; }
        public DailyRewardChestDescriptionLocParams LocParams { get; set; }
        public DateTimeOffset AvailableDate { get; set; }
    }

    public partial class DailyRewardChestDescriptionLocParams
    {
        public long Number1 { get; set; }
        public long? Number2 { get; set; }
    }

    public partial class WeeklyRewardChestDescription
    {
        public string Image1 { get; set; }
        public object Image2 { get; set; }
        public object Image3 { get; set; }
        public string Prefab { get; set; }
        public object ReferenceId { get; set; }
        public string HeaderLocKey { get; set; }
        public string DescriptionLocKey { get; set; }
        public long? Quantity { get; set; }
        public WeeklyRewardChestDescriptionLocParams LocParams { get; set; }
        public DateTimeOffset AvailableDate { get; set; }
    }

    public partial class WeeklyRewardChestDescriptionLocParams
    {
        public long Number1 { get; set; }
    }

    public partial class DeckSummary
    {
        public Guid DeckId { get; set; }
        public string Mana { get; set; }
        public string Name { get; set; }
        public Attribute[] Attributes { get; set; }
        public string Description { get; set; }
        public long DeckTileId { get; set; }
        public bool IsCompanionValid { get; set; }
        public Dictionary<string, bool> FormatLegalities { get; set; }
        public string CardBack { get; set; }
        public DeckValidationSummary[] DeckValidationSummaries { get; set; }
        public UnownedCards UnownedCards { get; set; }
    }

    //public partial class Attribute
    //{
    //    public Name Name { get; set; }
    //    public string Value { get; set; }
    //}

    public partial class DeckValidationSummary
    {
        public object Format { get; set; }
        public long InvalidCardCount { get; set; }
        public long NonFormatCardCount { get; set; }
        public long BannedCardCount { get; set; }
        public long UnownedCardCount { get; set; }
        public long DeckTooSmallCardCount { get; set; }
    }

    public partial class UnownedCards
    {
        public long? Rare { get; set; }
        public long? MythicRare { get; set; }
    }

    public partial class Format
    {
        public string Name { get; set; }
        public string[] Sets { get; set; }
        public long[] BannedTitleIds { get; set; }
        public long[] SuspendedTitleIds { get; set; }
        public long[] AllowedTitleIds { get; set; }
        public string CardCountRestriction { get; set; }
        public Quota MainDeckQuota { get; set; }
        public Quota SideBoardQuota { get; set; }
        public Quota CommandZoneQuota { get; set; }
    }

    public partial class Quota
    {
        public long Min { get; set; }
        public long Max { get; set; }
    }

    public partial class InventoryInfo
    {
        public long SeqId { get; set; }
        public object[] Changes { get; set; }
        public long Gems { get; set; }
        public long Gold { get; set; }
        public long TotalVaultProgress { get; set; }
        public long WcTrackPosition { get; set; }
        public long WildCardCommons { get; set; }
        public long WildCardUnCommons { get; set; }
        public long WildCardRares { get; set; }
        public long WildCardMythics { get; set; }
        public long DraftTokens { get; set; }
        public long SealedTokens { get; set; }
        public CustomTokens CustomTokens { get; set; }
        public Booster[] Boosters { get; set; }
        public Vouchers Vouchers { get; set; }
        public Cosmetics Cosmetics { get; set; }
    }

    //public partial class Booster
    //{
    //    public long CollationId { get; set; }
    //    public string SetCode { get; set; }
    //    public long Count { get; set; }
    //}

    public partial class CustomTokens
    {
        public long BattlePassAfrOrb { get; set; }
    }

    public partial class Vouchers
    {
    }

    public partial class KillSwitchNotification
    {
        public Vouchers KillSwitches { get; set; }
        public UxKillSwitches UxKillSwitches { get; set; }
    }

    public partial class UxKillSwitches
    {
        public bool Test { get; set; }
    }

    public partial class PreferredCosmetics
    {
        public Avatar Avatar { get; set; }
        public object Sleeve { get; set; }
        public Pet Pet { get; set; }
        public Emote[] Emotes { get; set; }
    }

    public partial class TokenDefinition
    {
        public string TokenId { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public long TokenType { get; set; }
        public object ThumbnailImageName { get; set; }
        public string PrefabName { get; set; }
        public object HeaderLocKey { get; set; }
        public string DescriptionLocKey { get; set; }
    }
}