using System.Linq;

namespace MTGAHelper.Entity
{
    public enum DeckCardZoneEnum
    {
        Deck,
        Sideboard,
        Commander,
    };

    public interface IDeckCard
    {
        //bool IsSideboard { get; }
        //int NbMissing { get; }
        //float MissingWeight { get; }
        string DisplayMember { get; }
        DeckCardZoneEnum Zone { get; }
    }

    public class DeckCard : CardWithAmount, IDeckCard
    {
        public DeckCardZoneEnum Zone { get; private set; }
        //public bool IsSideboard => Zone == DeckCardZoneEnum.Sideboard;

        public new string DisplayMember
        {
            get
            {
                var typeChar = Card.GetSimpleType()[0];
                //return $"[{typeChar}] {Amount}x {Card.name} (Missing {NbMissing})";
                return $"[{typeChar}] {Amount}x {Card.name}";
            }
        }

        public DeckCard(CardWithAmount card, DeckCardZoneEnum zone)
            : base(card.Card, card.Amount)
        {
            Zone = zone;
        }

        //public DeckCard(CardWithAmount card, bool isSideboard)
        //    : base(card.Card, card.Amount)
        //{
        //    //IsSideboard = isSideboard;
        //    Zone = isSideboard ? DeckCardZoneEnum.Deck : DeckCardZoneEnum.Sideboard;
        //}

        //public int NbMissing { get; private set; }

        //public float MissingWeight { get; private set; }

        //public void ApplyCompareResult(int nbMissing, float missingWeight)
        //{
        //    NbMissing = nbMissing;
        //    MissingWeight = missingWeight;
        //}
    }
}
