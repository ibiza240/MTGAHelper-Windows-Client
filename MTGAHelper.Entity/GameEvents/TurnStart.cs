using System;
using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Entity.GameEvents
{
    public class TurnStart : GameEventBase
    {
        public int TurnNumber { get; }
        public PlayerEnum ActivePlayer { get; }

        public TurnStart(DateTime localTime, int turnNumber, PlayerEnum activePlayer)
        {
            AtLocalTime = localTime;
            TurnNumber = turnNumber;
            ActivePlayer = activePlayer;
            Player = activePlayer;
        }

        public override string AsText => $"=====       Turn {TurnNumber}       =====";
    }
}
