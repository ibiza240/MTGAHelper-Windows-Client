
using MTGAHelper.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.Config
{
    public interface IConfigModel
    {
        string Id { get; }
    }

    [Serializable]
    public class ConfigModelDeck : ConfigModelRawDeck, IConfigModel
    {
        public const string SOURCE_SYSTEM = "automatic";
        public const string SOURCE_USERCUSTOM = "usercustom";
        public DateTime DateScrapedUtc { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public string ScraperTypeId { get; set; }
        public int? ScraperTypeOrderIndex { get; set; }
        public string Url { get; set; }
        public string UrlDeckList { get; set; }

        [JsonIgnore]
        public IDeck Deck { get; set; }

        public ConfigModelDeck()
        {
        }

        public ConfigModelDeck(IDeck deck, string url, DateTime dateCreatedUtc)
        {
            Deck = deck;
            ScraperTypeId = deck.ScraperType.Id;
            Id = Deck.Id;
            Name = deck.Name;
            DateScrapedUtc = DateTime.UtcNow;
            Url = url ?? "";
            DateCreatedUtc = dateCreatedUtc;
        }

        public ICollection<DeckCard> GetCards(Dictionary<int, Card> allCards)
        {
            //var test = CardsMain.Where(i => allCards.Any(x => x.grpId == i.Key) == false);

            var cardCommander = CardCommander == default(int) ? new DeckCard[0] : new[] { new DeckCard(new CardWithAmount(allCards[CardCommander], 1), DeckCardZoneEnum.Commander) };
            var cardsMain = CardsMain.Select(i => new DeckCard(new CardWithAmount(allCards[i.Key], i.Value), DeckCardZoneEnum.Deck));
            var cardsSideboard = CardsSideboard.Select(i => new DeckCard(new CardWithAmount(allCards[i.Key], i.Value), DeckCardZoneEnum.Sideboard));

            return cardCommander.Union(cardsMain).Union(cardsSideboard).ToArray();
        }
    }
}
