using MTGAHelper.Entity.DeckScraper;
using System;
using System.Linq;

namespace MTGAHelper.Entity
{
    public enum ScraperTypeEnum
    {
        Unknown,
        UserCustomDeck,
        MtgGoldfish,
        MtgDecks,
        Streamdecker,
        Aetherhub,
        Tappedout,
        MtgTop8,
        UserDeckSource,
        MtgaTool,
    }

    public enum ScraperTypeFormatEnum
    {
        Unknown,
        Standard,
        ArenaStandard,
    }


    public class ScraperType
    {
        public const string NAME_PREFIX_USER = "user_";

        public ScraperTypeEnum Type { get; private set; }
        public ScraperTypeFormatEnum Format { get; private set; }
        public string Name { get; private set; }
        public bool IsByUser { get; private set; }

        public string Id => $"{Type.ToString().ToLower()}-{Name?.ToLower() ?? "NULL"}{(Format == ScraperTypeFormatEnum.Unknown ? "" : "-" + Format.ToString().ToLower())}";

        public string Url
        {
            get
            {
                Func<string, ScraperTypeFormatEnum, string> aetherhubAddFormat = (s, f) =>
                s + (f == ScraperTypeFormatEnum.Standard ? "/Standard" : f == ScraperTypeFormatEnum.ArenaStandard ? "/Arena%20Standard" : "");

                Func<ScraperTypeFormatEnum, string> mtgGoldfishAddFormat = (f) =>
                f == ScraperTypeFormatEnum.Standard ? "standard" : f == ScraperTypeFormatEnum.ArenaStandard ? "arena_standard" : "";

                var url = "#";
                switch (Type)
                {
                    case ScraperTypeEnum.Streamdecker:
                        url = "https://www.streamdecker.com/decks/" + Name;
                        break;
                    case ScraperTypeEnum.Aetherhub:
                        if (Name == AetherhubListingEnum.Meta.ToString().ToLower())
                            url = aetherhubAddFormat("https://aetherhub.com/Meta/Format", Format);
                        else if (Name == AetherhubListingEnum.Tier1.ToString().ToLower())
                            url = "https://aetherhub.com/Meta/TierOne";
                        else if (Name.StartsWith(NAME_PREFIX_USER))
                        {
                            var id = new string(Name.Skip(NAME_PREFIX_USER.Length).ToArray());
                            url = aetherhubAddFormat($"https://aetherhub.com/User/{id}/Decks", Format);
                        }
                        break;
                    case ScraperTypeEnum.MtgGoldfish:
                        if (Name == MtgGoldfishArticleEnum.Meta.ToString().ToLower())
                            url = $"https://www.mtggoldfish.com/metagame/{mtgGoldfishAddFormat(Format)}/full";
                        else if (Name == MtgGoldfishArticleEnum.AgainstTheOdds.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/series/against-the-odds";
                        else if (Name == MtgGoldfishArticleEnum.BudgetMagic.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/series/budget-magic";
                        else if (Name == MtgGoldfishArticleEnum.GoldfishGladiators.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?tag=goldfish+gladiators";
                        else if (Name == MtgGoldfishArticleEnum.InstantDeckTech.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?tag=instant+deck+tech";
                        else if (Name == MtgGoldfishArticleEnum.FishFiveO.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?tag=fish+five-o";
                        else if (Name == MtgGoldfishArticleEnum.MuchAbrew.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/series/much-abrew-about-nothing";
                        else if (Name == MtgGoldfishArticleEnum.StreamHighlights.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?tag=stream+highlights";
                        else if (Name == MtgGoldfishArticleEnum.BudgetArena.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?tag=budget+arena";
                        else if (Name == MtgGoldfishArticleEnum.SingleScoop.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/articles/search?author=93";
                        else if (Name == MtgGoldfishArticleEnum.Tournaments.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/tournaments/standard";

                        break;
                    case ScraperTypeEnum.MtgDecks:
                        //switch (Name)
                        //{
                        //    case "meta":
                        //        Url = "https://mtgdecks.net/Standard";
                        //        break;
                        //    case "averagearchetype":
                        //        Url = "https://mtgdecks.net/Standard";
                        //        break;
                        //}
                        url = "https://mtgdecks.net/Standard";
                        break;
                    case ScraperTypeEnum.MtgaTool:
                        var format = Name == MtgaToolFormatEnum.Bo1.ToString().ToLower() ? "Bo1" : "Bo2";
                        url = "http://mtgatool.com/metagame/{format}";
                        break;

                    // WHAT?!????????????
                    case ScraperTypeEnum.MtgTop8:
                        if (Name == MtgTop8ListingEnum.DecksToBeat.ToString().ToLower())
                            url = "https://www.mtggoldfish.com/metagame/{mtgGoldfishAddFormat(Format)}/full";
                        break;
                }

                return url;
            }
        }

        public override string ToString() => Id;

        public ScraperType(ScraperTypeEnum type, string name, ScraperTypeFormatEnum format = ScraperTypeFormatEnum.Unknown)
        {
            Type = type;
            Name = name;
            Format = format;
            SetIsByUser();
        }

        public ScraperType(string id)
        {
            if (id.IndexOf('-') < 1)
            {
                var strType = id.Trim('-');
                Type = DecodeType(strType);
                Name = null;
            }
            else
            {
                var parts = id.Split('-');
                Type = DecodeType(parts[0]);
                Name = parts[1];

                if (parts.Length > 2)
                {
                    Format = (ScraperTypeFormatEnum)Enum.Parse(typeof(ScraperTypeFormatEnum), parts[2], true);
                }
            }
            SetIsByUser();
        }

        void SetIsByUser()
        {
            IsByUser = Name != null && (Name.StartsWith(NAME_PREFIX_USER) || Type == ScraperTypeEnum.Streamdecker);

            //if (IsByUser) System.Diagnostics.Debugger.Break();
        }

        private ScraperTypeEnum DecodeType(string strType)
        {
            if (Enum.TryParse(strType, true, out ScraperTypeEnum e))
                return e;

            return ScraperTypeEnum.Unknown;
        }
    }

}
