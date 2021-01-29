using System;
using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Entity.GameEvents
{
    public class Scry : GameEventBase
    {
        public int ToTop { get; }
        public int ToBottom { get; }

        public Scry(DateTime localTime, PlayerEnum player, int toTop, int toBottom)
        {
            AtLocalTime = localTime;
            Player = player;
            ToTop = toTop;
            ToBottom = toBottom;
        }

        public override string AsText => $"Scry: {ToTop} top; {ToBottom} bottom.";
    }
}
