using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public enum CardForTurnEnum
    {
        Unknown,
        Drew,
        Played,
        Discarded,
        SpellResolved,
        PermanentRemoved,
        FromLibraryToBattlefield,
        FromGraveyardToBattlefield,
        FromGraveyardToHand,
        FromLibraryToExile,
        FromExileToStack,
        FromExileToBattlefield,
        FromLibraryToGraveyard,
        FromExileToHand,
        FromGraveyardToStack,
        FromGraveyardToExile,
        Conceded
    }

    public enum PlayerEnum
    {
        Unknown,
        Me,
        Opponent,
    }

    public class CardTurnAction
    {
        public int Turn { get; set; }
        public PlayerEnum Player { get; set; }
        public CardForTurnEnum Action { get; set; }
        public int CardGrpId { get; set; }
    }

    public class GameDetail
    {
        public DateTime StartDateTime { get; set; }
        public long SecondsCount { get; set; }
        public GameOutcomeEnum Outcome { get; set; }
        public FirstTurnEnum FirstTurn { get; set; }
        public int MulliganCount => StartingHands.Count - 1;
        public int MulliganCountOpponent { get; set; }

        //public ConfigModelRawDeck DeckUsed { get; set; }
        public Dictionary<int, int> DeckCards { get; set; }

        public Dictionary<int, int> OpponentCardsSeen { get; set; } = new Dictionary<int, int>();
        public IList<ICollection<int>> StartingHands { get; set; } = new List<ICollection<int>>();
        public ICollection<CardTurnAction> CardTransfers { get; set; } = new CardTurnAction[0];
    }
}
