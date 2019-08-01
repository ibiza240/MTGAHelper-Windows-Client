namespace MTGAHelper.Entity
{
    public class CardWithAmount
    {
        public Card Card { get; set; }
        public int Amount { get; private set; }

        public CardWithAmount(Card card)
        {
            Card = card;
        }

        public CardWithAmount(Card card, int amount)
        {
            Card = card;
            Amount = amount;
        }

        public string DisplayMember { get { return $"{Amount}x {Card.name}"; } }

        public override string ToString()
        {
            return $"{Amount}x {Card.name}";
        }
    }

    public class CardForDraftPick : Card
    {
        public string Rating { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }
        public int Top5Rank { get; set; }
    }
}
