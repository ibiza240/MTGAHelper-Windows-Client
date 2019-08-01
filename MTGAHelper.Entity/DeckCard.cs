using System.Linq;

namespace MTGAHelper.Entity
{
    public interface IDeckCard
    {
        bool IsSideboard { get; }
        //int NbMissing { get; }
        //float MissingWeight { get; }
        string DisplayMember { get; }
    }

    public class DeckCard : CardWithAmount, IDeckCard
    {
        public DeckCard(CardWithAmount card, bool isSideboard)
            : base(card.Card, card.Amount)
        {
            IsSideboard = isSideboard;
        }

        public bool IsSideboard { get; private set; }

        //public int NbMissing { get; private set; }

        //public float MissingWeight { get; private set; }

        public new string DisplayMember
        {
            get
            {
                var typeChar = Card.GetSimpleType()[0];
                //return $"[{typeChar}] {Amount}x {Card.name} (Missing {NbMissing})";
                return $"[{typeChar}] {Amount}x {Card.name}";
            }
        }

        //public void ApplyCompareResult(int nbMissing, float missingWeight)
        //{
        //    NbMissing = nbMissing;
        //    MissingWeight = missingWeight;
        //}
    }
}
