using System;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.Entity.GameEvents
{
    public class CastSpell : GameEventBase
    {
        public CastSpell(DateTime localTime, PlayerEnum player, Card card)
        {
            AtLocalTime = localTime;
            Player = player;
            Card = card;
        }

        public override string AsText => $"{PlayStr()} '{Card.name}'.";

        private string PlayStr()
        {
            return Player == PlayerEnum.Me ? "You cast" : "Opponent casts";
        }
    }
}
