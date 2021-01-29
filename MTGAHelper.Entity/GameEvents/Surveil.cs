using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.Entity.GameEvents
{
    public class Surveil : GameEventBase
    {
        public int LookedAt { get; }
        public int ToTop { get; }

        [JsonIgnore]
        public ICollection<Card> ToGraveyard { get; }

        public Surveil(DateTime localTime, PlayerEnum player, int lookedAt, int toTop, ICollection<Card> toGraveyard)
        {
            AtLocalTime = localTime;
            Player = player;
            LookedAt = lookedAt;
            ToTop = toTop;
            ToGraveyard = toGraveyard;
        }

        public override string AsText => $"Surveil {LookedAt}: {ToTop} top; {ToGraveyard.Count} to graveyard ({string.Join("; ", ToGraveyard.Select(c => c.name))}).";
    }
}
