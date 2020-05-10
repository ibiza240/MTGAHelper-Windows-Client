using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal interface IZoneTracker
    {
        IEnumerable<CardDrawInfo> GrpIdInfos { get; }
        void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards);
        void AddCards(IReadOnlyCollection<StateCard2> cardsToAdd);
        IReadOnlyCollection<StateCard2> TakeCards(IReadOnlyCollection<ITrackedCard> cardsToTake);
        bool Clear();
    }
}