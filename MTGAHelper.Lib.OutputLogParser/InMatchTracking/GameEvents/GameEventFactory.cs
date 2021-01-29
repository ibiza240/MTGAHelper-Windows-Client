using System.Collections.Generic;
using MTGAHelper.Entity.GameEvents;
using MTGAHelper.Entity.MtgaOutputLog;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking.GameEvents
{
    public class GameEventFactory
    {
        private readonly ITimeProvider timeProvider;

        public GameEventFactory(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        internal IGameEvent FromZoneTransfer(ZoneTransferInfo2 zt)
        {
            var t = timeProvider.Now;
            var p = zt.SrcZone.GetPlayerEnum();
            var c = zt.Card;
            switch (zt.Category)
            {
                case "CastSpell":
                    return new CastSpell(t, p, c);

                case "Discard":
                    return new DiscardCard(t, p, c);

                case "Draw":
                    return new DrawCard(t, p, c);

                case "PlayLand":
                    return new PlayLand(t, p, c);

                case "Resolve":
                    Log.Information("Spell resolved: {zoneTransferInfo}", zt);
                    break;
            }

            return null;
        }

        public IGameEvent FromTopBottom(bool foundInLib, IReadOnlyCollection<int> topIdsCount, IReadOnlyCollection<int> bottomIdsCount)
        {
            return new Scry(
                timeProvider.Now,
                foundInLib ? PlayerEnum.Me : PlayerEnum.Opponent,
                topIdsCount?.Count ?? 0,
                bottomIdsCount?.Count ?? 0);
        }

        public IGameEvent StartTurn(int turnNumber, PlayerEnum activePlayer)
        {
            return new TurnStart(timeProvider.Now, turnNumber, activePlayer);
        }
    }
}
