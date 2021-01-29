using System;
using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Entity.GameEvents
{
    public interface IGameEvent
    {
        DateTime AtLocalTime { get; }
        PlayerEnum Player { get; }
        string AsText { get; }
        Card Card { get; }
    }

    public static class GameEventExtensions
    {
        public static string StringRep(this IGameEvent e)
        {
            return e == null ? "null" : $"{e.AtLocalTime:T}: {e.AsText} ({e.Player})";
        }
    }

    public abstract class GameEventBase : IGameEvent
    {
        public DateTime AtLocalTime { get; protected set; }

        public PlayerEnum Player { get; protected set; }

        public abstract string AsText { get; }

        /// <summary>
        /// Can be null if the event is not related to a card
        /// </summary>
        public Card Card { get; protected set; }
    }

    public class OutputLogResultGameEvent
    {
        public PlayerEnum Player { get; set; }

        public string AsText { get; set; }

        public int CardGrpId { get; set; }
    }
}
