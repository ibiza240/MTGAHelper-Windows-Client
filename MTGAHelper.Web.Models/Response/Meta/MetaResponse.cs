using MTGAHelper.Entity.Archetypes;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.Config.Decks;

namespace MTGAHelper.Web.UI.Model.Response.Meta
{
    public class MetaResponseArchetype
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int NbDecks { get; set; }
        //public ICollection<MetaResponseDeck> Decks { get; set; }
    }

    //public class MetaResponseDeck
    //{
    //    public string Name { get; set; }
    //    public string Id { get; set; }
    //    public int? Order { get; set; }
    //    public ICollection<string> Similar { get; set; }
    //}

    //public class MetaResponseCard
    //{
    //    public string Name { get; set; }
    //}

    public class MetaResponse
    {
        public ICollection<MetaResponseArchetype> Archetypes { get; set; }

        //public MetaResponse(DecksAnalyzerResult result, UtilColors utilColors)
        //{
        //    Archetypes = result.Archetypes
        //        .Select(i => new MetaResponseArchetype
        //        {
        //            Color = utilColors.FromDeck(i.Decks.First().ConfigDeck.Deck),
        //            Decks = i.Decks
        //                .Select(x => new MetaResponseDeck
        //                {
        //                    Id = x.ConfigDeck.Id,
        //                    Name = x.ConfigDeck.Name,
        //                    Order = x.ConfigDeck.ScraperTypeOrderIndex,
        //                    Similar = x.Similars.Select(y => $"{y.Deck.Name} ({y.Similarity.ToString("0.00")})").ToArray()
        //                })
        //                .ToArray()
        //        })
        //        .ToArray();
        //}
        public MetaResponse(ICollection<ConfigModelDeck> decks)
        {
            var dictArchetypes = Archetype.DefaultList.ToDictionary(i => i.Id, i => i);

            Archetypes = decks
                .Where(i => i.ArchetypeId != null)
                .GroupBy(i => i.ArchetypeId)
                .Select(i => new MetaResponseArchetype
                {
                    Name = dictArchetypes[i.Key].Name,
                    Color = dictArchetypes[i.Key].Color,
                    NbDecks = i.Count(),
                })
                .ToArray();
        }
    }
}
