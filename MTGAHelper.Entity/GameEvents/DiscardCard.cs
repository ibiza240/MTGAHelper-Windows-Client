using System;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.Entity.GameEvents
{
    public class DiscardCard : GameEventBase
    {
        public DiscardCard(DateTime localTime, PlayerEnum player, Card card)
        {
            AtLocalTime = localTime;
            Player = player;
            Card = card;
        }

        public override string AsText => $"{DiscardStr()} card: {Card.name}.";

        private string DiscardStr()
        {
            return Player == PlayerEnum.Me ? "You discard" : "Opponent discards";
        }
    }
}
