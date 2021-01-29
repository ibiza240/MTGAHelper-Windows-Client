using MTGAHelper.Entity.GameEvents;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class ActionLogEntry
    {
        private readonly IGameEvent _gameEvent;

        public ActionLogEntry(IGameEvent gameEvent)
        {
            _gameEvent = gameEvent;
            Text = gameEvent.AsText;
        }

        public string Text { get; }
        public string Time => $"{_gameEvent.AtLocalTime.ToLongTimeString()}: ";

        public override string ToString()
        {
            return Text;
        }
    }
}