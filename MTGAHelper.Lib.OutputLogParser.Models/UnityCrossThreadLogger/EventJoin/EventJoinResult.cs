using MTGAHelper.Entity.OutputLogParsing;
using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventJoin
{
    public class EventJoinResult : MtgaOutputLogPartResultBase<PayloadRaw2<EventJoinPayloadRaw>>
    {
    }

    public partial class EventJoinPayloadRaw
    {
        public long SeqId { get; set; }
        public Change[] Changes { get; set; }
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

        //public CustomTokens CustomTokens { get; set; }
        public Booster[] Boosters { get; set; }

        //public CustomTokens Vouchers { get; set; }
        //public Cosmetics Cosmetics { get; set; }
    }

    public partial class Booster
    {
        public long CollationId { get; set; }
        public string SetCode { get; set; }
        public long Count { get; set; }
    }

    public partial class Change
    {
        public string Source { get; set; }
        public Guid SourceId { get; set; }
        public long InventoryGold { get; set; }
        public long InventoryGems { get; set; }
        public long InventoryDraftTokens { get; set; }
        public long InventorySealedTokens { get; set; }

        //public CustomTokens InventoryCustomTokens { get; set; }
        public object[] ArtStyles { get; set; }

        public object[] Avatars { get; set; }
        public object[] Sleeves { get; set; }
        public object[] Pets { get; set; }
        public object[] Emotes { get; set; }
        public long WildCardTrackUnCommons { get; set; }
        public long WildCardTrackRares { get; set; }
        public long WildCardTrackMythics { get; set; }
        public long SetMasteryProgress { get; set; }
        public long WildCardCommons { get; set; }
        public long WildCardUnCommons { get; set; }
        public long WildCardRares { get; set; }
        public long WildCardMythics { get; set; }
        public object[] Decks { get; set; }

        //public CustomTokens DeckCards { get; set; }
        public object[] Boosters { get; set; }

        public object[] GrantedCards { get; set; }
        //public CustomTokens Vouchers { get; set; }
    }

    //public partial class CustomTokens
    //{
    //}

    //public partial class Cosmetics
    //{
    //    public ArtStyle[] ArtStyles { get; set; }
    //    public Avatar[] Avatars { get; set; }
    //    public ArtStyle[] Pets { get; set; }
    //    public Avatar[] Sleeves { get; set; }
    //    public Emote[] Emotes { get; set; }
    //}

    //public partial class ArtStyle
    //{
    //    public ArtStyleType Type { get; set; }
    //    public string Id { get; set; }
    //    public long? ArtId { get; set; }
    //    public Variant Variant { get; set; }
    //    public string Name { get; set; }
    //}

    //public partial class Avatar
    //{
    //    public string Id { get; set; }
    //    public AvatarType Type { get; set; }
    //}

    //public partial class Emote
    //{
    //    public string Id { get; set; }
    //    public EmoteType Type { get; set; }
    //    public Page Page { get; set; }
    //    public FlipType FlipType { get; set; }
    //    public string Category { get; set; }
    //    public string Treatment { get; set; }
    //}

    //public enum ArtStyleType { ArtStyle, Pet };

    //public enum Variant { Da, DaAa, Ff, Level1, Level2, Level3, Rlb, Sh, Skin1, Skin2, Skin3, Skin4, Skin5, Toho };

    //public enum AvatarType { Avatar, Sleeve };

    //public enum FlipType { None, Priority, Reply };

    //public enum Page { Phrase, Sticker };

    //public enum EmoteType { Emote };
}