using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class GameDetailDto
    {
        public DateTime StartDateTime { get; set; }
        public long SecondsCount { get; set; }
        public string Outcome { get; set; }
        public string FirstTurn { get; set; }
        public int MulliganCount { get; set; }
        public int MulliganCountOpponent { get; set; }

        //public ConfigModelRawDeck DeckUsed { get; set; }
        public Dictionary<int, int> DeckCards { get; set; }

        public ICollection<CardWithAmountDto> OpponentCardsSeen { get; set; }
        public IList<ICollection<CardDto>> StartingHands { get; set; }
        public ICollection<CardTurnActionDto> CardTransfers { get; set; }
    }
}
