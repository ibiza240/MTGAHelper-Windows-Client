using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public class DeckAverageArchetype : DeckBase
    {
        public ICollection<DeckAverageArchetypeOtherMainCard> CardsMainOther { get; set; }

        public DeckAverageArchetype(string name, ScraperType scraperType, IEnumerable<DeckCard> cardsMain, IEnumerable<Card> cardsMainOther, IEnumerable<Card> cardsSideboard)
            : base(name, scraperType)
        {
            Cards = new DeckCards(cardsMain.Select(i => new DeckCard(i, DeckCardZoneEnum.Deck))
                .Union(cardsSideboard.Select(i => new DeckCard(new CardWithAmount(i, 1), DeckCardZoneEnum.Sideboard)))
                .ToArray());

            CardsMainOther = cardsMainOther.Select(i => new DeckAverageArchetypeOtherMainCard(i, 0)).ToArray();
        }

        //public override void ApplyCompareResult(Card card, bool isSideboard, int nbMissing, float missingWeight, int nbOwned)
        //{
        //    //var cardInDeck = Cards.SingleOrDefault(i => i.Card == card && i.IsSideboard == isSideboard);
        //    //if (cardInDeck != null)
        //    //{
        //    //    cardInDeck.ApplyCompareResult(nbMissing, missingWeight);
        //    //}

        //    //if (isSideboard == false)
        //    //{
        //    //    var cardMainOther = CardsMainOther.SingleOrDefault(i => i == card);
        //    //    if (cardMainOther != null)
        //    //    {
        //    //        // card comes from the "Other cards in Main"
        //    //        cardMainOther.ApplyCompareResult(nbOwned);
        //    //    }
        //    //}
        //    var cardMainOther = CardsMainOther?.SingleOrDefault(i => i == card);
        //    if (cardMainOther != null)
        //        // card comes from the "Other cards in Main"
        //        cardMainOther.ApplyCompareResult(nbOwned);
        //    else
        //        Cards.Single(i => i.Card == card && i.IsSideboard == isSideboard).ApplyCompareResult(nbMissing, missingWeight);
        //}

        //public override float CalcMissingWeight()
        //{
        //    return Cards.Sum(i => i.MissingWeight);
        //}
    }
}
