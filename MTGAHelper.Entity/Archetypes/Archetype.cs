using System.Collections.Generic;

namespace MTGAHelper.Entity.Archetypes
{
    public class Archetype
    {
        public static readonly ICollection<Archetype> DefaultList = new[]
        {
            new Archetype
            {
                Set = "M20",
                Name = "Boros Feather",
                Variant = "Feather",
                Color = "WR",
                Cards = new [] { "Feather, the Redeemed" }
            },
            new Archetype
            {
                Set = "M20",
                Name = "Naya Feather",
                Variant = "Feather",
                Color = "WRG",
                Cards = new [] { "Feather, the Redeemed" }
            },
            new Archetype
            {
                Set = "M20",
                Name = "4 colors Kethis",
                Variant = "Kethis",
                Color = "WUBG",
                Cards = new [] { "Kethis, the Hidden Hand", "Lazav, the Multifarious" }
            },
            new Archetype
            {
                Set = "M20",
                Name = "Vampires",
                Variant = "Vampires",
                Color = "WB",
                Cards = new [] { "Sorin, Imperious Bloodlord" }
            },
            new Archetype
            {
                Set = "M20",
                Name = "Red Aggro",
                Variant = "Red Aggro",
                Color = "R",
                Cards = new [] { "Light Up the Stage", "Shock", "Lightning Strike" }
            },
            new Archetype
            {
                Set = "M20",
                Name = "Esper Control",
                Variant = "Esper Control",
                Color = "WUB",
                Cards = new [] { "Teferi, Hero of Dominaria", "Teferi, Time Raveler", "Kaya's Wrath" }
            },
        };

        public string Set { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public ICollection<string> Cards { get; set; }
        public ICollection<string> CardsExcluded { get; set; } = new string[0];

        /// <summary>
        /// Like Name, but more generic. Idea is many archetypes can be the same Variant
        /// </summary>
        public string Variant { get; set; }

        public string Id => $"{Set}-{Name}";
    }
}
