using MTGAHelper.Lib.Analyzers.Cards;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response.Meta
{
    public class MetaResponseArchetype
    {
        public string Color { get; set; }
        public ICollection<MetaResponseDeck> Decks { get; set; }
    }

    public class MetaResponseDeck
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int? Order { get; set; }
        public ICollection<string> Similar { get; set; }
    }

    public class MetaResponseCard
    {
        public string Name { get; set; }
    }

    public class MetaResponse
    {
        public ICollection<MetaResponseArchetype> Archetypes { get; set; }

        public MetaResponse(DecksAnalyzerResult result)
        {
            Archetypes = result.Archetypes
                .Select(i => new MetaResponseArchetype
                {
                    Color = i.Decks.First().ConfigDeck.Deck.GetColor(),
                    Decks = i.Decks
                        .Select(x => new MetaResponseDeck
                        {
                            Id = x.ConfigDeck.Id,
                            Name = x.ConfigDeck.Name,
                            Order = x.ConfigDeck.ScraperTypeOrderIndex,
                            Similar = x.Similars.Select(y => $"{y.Deck.Name} ({y.Similarity.ToString("0.00")})").ToArray()
                        })
                        .ToArray()
                })
                .ToArray();
        }
    }
}
