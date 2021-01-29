using System;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.Entity.GameEvents
{
    public class DrawCard : GameEventBase
    {
        public DrawCard(DateTime localTime, PlayerEnum player, Card card)
        {
            AtLocalTime = localTime;
            Player = player;
            Card = card;
        }

        public override string AsText => Player == PlayerEnum.Me ? $"You draw card: {Card.name}." : "Opponent draws a card.";
    }
}
